#include <stdbool.h>
#include <math.h>
#include "main.h"
#include "LED_Handler.h"
#include "LP5024_Driver.h"

#define CATEGORY_SIZE 15
#define HUE_MAX 360
#define HUE_STEP 15
#define SPEED_MIN 0.001f // corresponds to patternSpeed = 0
#define SPEED_MAX 0.1f // corresponds to patternSpeed = 100

// Definitions of idle and button press lighting configurations
LightingConfiguration idleLightingConfig;
LightingConfiguration buttonPressLightingConfig;
LightingConfiguration lidLiftLightingConfig;
ButtonConfiguration buttonConfig;

// Pointer to the active lighting configuration
LightingConfiguration* activeLightingConfig = &idleLightingConfig;

bool startupSettingsRecieved = false;
volatile int hue = 0;
volatile float t = 0.0f;
volatile int transitionDirection = 1;
/**
 * @brief Initialize the lighting settings for idle and button press.
 * Set the default parameters for both configurations.
 */
void initLightingConfig(void) {
    // Default settings for the idle light on powerup
    idleLightingConfig.settingChanged = false;
    idleLightingConfig.pattern = PATTERN_NONE;
    idleLightingConfig.brightness = 0x10;
    idleLightingConfig.patternSpeed = 0x32;
    idleLightingConfig.frameColor1 = 0xFFFFFF;
    idleLightingConfig.frameColor2 = 0xFFFFFF;
    idleLightingConfig.buttonColor1 = 0xFFFFFF;
    idleLightingConfig.buttonColor2 = 0xFFFFFF;

    // Default settings for the button press lighting on powerup
    buttonPressLightingConfig.settingChanged = false;
    buttonPressLightingConfig.pattern = PATTERN_NONE;
    buttonPressLightingConfig.brightness = 0x10;
    buttonPressLightingConfig.patternSpeed = 0x32;
    buttonPressLightingConfig.frameColor1 = 0xFFFFFF;
    buttonPressLightingConfig.frameColor2 = 0xFFFFFF;
    buttonPressLightingConfig.buttonColor1 = 0xFFFFFF;
    buttonPressLightingConfig.buttonColor2 = 0xFFFFFF;

    // Default settings for the lid lift lighting on powerup
    lidLiftLightingConfig.settingChanged = false;
    lidLiftLightingConfig.pattern = PATTERN_NONE;
    lidLiftLightingConfig.brightness = 0x10;
    lidLiftLightingConfig.patternSpeed = 0x32;
    lidLiftLightingConfig.frameColor1 = 0xFFFFFF;
    lidLiftLightingConfig.frameColor2 = 0xFFFFFF;
    lidLiftLightingConfig.buttonColor1 = 0xFFFFFF;
    lidLiftLightingConfig.buttonColor2 = 0xFFFFFF;

    // Default setting for time duration for button press lighting
    buttonConfig.lightDuration = 30;
}

void updateIdleLightingConfig(LightingPattern pattern, uint8_t brightness, uint8_t patternSpeed, uint32_t frameColor1, uint32_t frameColor2, uint32_t buttonColor1, uint32_t buttonColor2)
{
    idleLightingConfig.pattern = pattern;
    idleLightingConfig.brightness = brightness;
    idleLightingConfig.patternSpeed = patternSpeed;
    idleLightingConfig.frameColor1 = frameColor1;
    idleLightingConfig.frameColor2 = frameColor2;
    idleLightingConfig.buttonColor1 = buttonColor1;
    idleLightingConfig.buttonColor2 = buttonColor2;
}

void updateButtonPressLightingConfig(LightingPattern pattern, uint8_t brightness, uint8_t patternSpeed, uint32_t frameColor1, uint32_t frameColor2, uint32_t buttonColor1, uint32_t buttonColor2)
{
    buttonPressLightingConfig.pattern = pattern;
    buttonPressLightingConfig.brightness = brightness;
    buttonPressLightingConfig.patternSpeed = patternSpeed;
    buttonPressLightingConfig.frameColor1 = frameColor1;
    buttonPressLightingConfig.frameColor2 = frameColor2;
    buttonPressLightingConfig.buttonColor1 = buttonColor1;
    buttonPressLightingConfig.buttonColor2 = buttonColor2;
}

/**
 * @brief Update bulk light settings
 * 
 * This function updates the lighting configuration for different lighting modes using the provided report buffer. 
 * It expects a buffer in a specific format where the first byte is the BULK_SETTINGS_LOAD command 
 * and the rest are the setting values for each lighting mode, each setting consisting of a specific pattern, 
 * brightness, pattern speed, frame color 1 and 2, button color 1 and 2. Each category's values are sequentially
 * ordered in the buffer.
 * 
 * The function will update the global lighting configuration variables idleLightingConfig, buttonPressLightingConfig, 
 * and lidLiftLightingConfig, and will set the settingChanged flag to true to indicate that the settings have been updated.
 * 
 * @param report_buffer A pointer to the buffer containing the light settings report
 * @param buffer_length The length of the report buffer
 */
void updateBulkLightSettings(uint8_t *report_buffer, uint16_t buffer_length)
{
  // Increment report_buffer pointer by 1 to skip the BULK_SETTINGS_LOAD command report byte
  report_buffer += 1;

  // Update LightingConfiguration for each lighting mode
  LightingConfiguration* configs[] = {&idleLightingConfig, &buttonPressLightingConfig, &lidLiftLightingConfig};

  for (int i = 0; i < sizeof(configs)/sizeof(configs[0]); i++) {
    configs[i]->pattern = report_buffer[0];
    configs[i]->brightness = report_buffer[1];
    configs[i]->patternSpeed = report_buffer[2];
    configs[i]->frameColor1 = (report_buffer[3] << 16) | (report_buffer[4] << 8) | report_buffer[5];
    configs[i]->frameColor2 = (report_buffer[6] << 16) | (report_buffer[7] << 8) | report_buffer[8];
    configs[i]->buttonColor1 = (report_buffer[9] << 16) | (report_buffer[10] << 8) | report_buffer[11];
    configs[i]->buttonColor2 = (report_buffer[12] << 16) | (report_buffer[13] << 8) | report_buffer[14];
  
    configs[i]->settingChanged = true;

    // Move to the start of the next category in buffer
    report_buffer += CATEGORY_SIZE;
  }

  buttonConfig.lightDuration = report_buffer[0];
}

/**
 * @brief Update the lighting configuration based on the report type and value.
 * @param config: The lighting configuration to update.
 * @param report: The type of report which determines what part of the configuration to update.
 * @param value: The new value to set in the configuration.
 */
void UpdateLightingConfiguration(LightingConfiguration* config, ReportLightEnum report, uint32_t value) {
  // Depending on the report type, update the corresponding configuration parameter
  switch(report) {
    case REPORT_PATTERN:
      config->pattern = (LightingPattern)value;
      break;
    case REPORT_PATTERN_SPEED:
      if(value > 100)
        value = 100;
      config->patternSpeed = value;
      break;
    case REPORT_BRIGHTNESS:
      if(value > 100)
        value = 100;
      config->brightness = value;
      break;
    case REPORT_FRAME_COLOR1:
      if(value > 0xFFFFFF)
        value = 0xFFFFFF;
      config->frameColor1 = value;
      break;
    case REPORT_FRAME_COLOR2:
    if(value > 0xFFFFFF)
        value = 0xFFFFFF;
      config->frameColor2 = value;
      break;
    case REPORT_BUTTON_COLOR1:
    if(value > 0xFFFFFF)
        value = 0xFFFFFF;
      config->buttonColor1 = value;
      break;
    case REPORT_BUTTON_COLOR2:
    if(value > 0xFFFFFF)
        value = 0xFFFFFF;
      config->buttonColor2 = value;
      break;
    default:
      // Unknown report type
      break;
  }
  // Indicate that the configuration has changed
  config->settingChanged = true;
}

/**
 * @brief Update the button configuration based on the report type and value.
 * @param config: The button configuration to update.
 * @param report: The type of report which determines what part of the configuration to update.
 * @param value: The new value to set in the configuration.
 */
void UpdateButtonConfiguration(ButtonConfiguration* config, ReportBtnEnum report, uint32_t value) {
  // Depending on the report type, update the corresponding configuration parameter
  switch(report) {
    case REPORT_BUTTON_DURATION:
      config->lightDuration = value;
      break;
    default:
      // Unknown report type
      break;
  }
}

float lerp(float a, float b, float t) {
    return a + t * (b - a);
}

HAL_StatusTypeDef LightingHandler(void)
{
  HAL_StatusTypeDef status = HAL_OK;
  int r, g, b;
  int color1, color2, interpolatedColor;
  float speed, hueIncrement;

  if (activeLightingConfig->settingChanged == true) {
    activeLightingConfig->settingChanged = false;
  }

  switch(activeLightingConfig->pattern) {
    case PATTERN_NONE:
      LP5024_SetColor_All(0);
      break;
      
    case PATTERN_STATIC:
      status = LP5024_SetColor(LED7, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED6, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED5, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED4, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED3, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED2, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED1, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED0, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      break;
    case PATTERN_BLINK:
      // Update t based on the transition direction
      speed = SPEED_MIN + (SPEED_MAX - SPEED_MIN) * activeLightingConfig->patternSpeed / 100.0f;
      t += speed * transitionDirection; // Adjust this value to control the speed and direction of the transition

      // Check if t reaches the limits and handle accordingly
      if (t > 1.0f) {
        t = 1.0f; // Set t to 1.0f for a smooth transition without blink
        transitionDirection = -1; // Reverse the transition direction
        status = LP5024_SetColor(LED7, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED6, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED5, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED4, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED3, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED2, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED1, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED0, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
      } else if (t < 0.0f) {
        t = -t; // Calculate the absolute value of t
        transitionDirection = 1; // Reverse the transition direction
        LP5024_SetColor_All(0);
      }
      break;

    case PATTERN_BLINK_BETWEEN:
      // Update t based on the transition direction
      speed = SPEED_MIN + (SPEED_MAX - SPEED_MIN) * activeLightingConfig->patternSpeed / 100.0f;
      t += speed * transitionDirection; // Adjust this value to control the speed and direction of the transition

      // Check if t reaches the limits and handle accordingly
      if (t > 1.0f) {
        t = 1.0f; // Set t to 1.0f for a smooth transition without blink
        transitionDirection = -1; // Reverse the transition direction
        
        status = LP5024_SetColor(LED7, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED6, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED5, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED4, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED3, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED2, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED1, Adjust_Color_Brightness(activeLightingConfig->frameColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED0, Adjust_Color_Brightness(activeLightingConfig->buttonColor1, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
      } else if (t < 0.0f) {
        t = -t; // Calculate the absolute value of t
        transitionDirection = 1; // Reverse the transition direction

        status = LP5024_SetColor(LED7, Adjust_Color_Brightness(activeLightingConfig->buttonColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED6, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED5, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED4, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED3, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED2, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED1, Adjust_Color_Brightness(activeLightingConfig->frameColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
        status = LP5024_SetColor(LED0, Adjust_Color_Brightness(activeLightingConfig->buttonColor2, activeLightingConfig->brightness));
        if(status != HAL_OK) return status;
      }
      break;

    case PATTERN_EASE_IN:
      break;

    case PATTERN_EASE_BETWEEN:
      // Interpolate between the two colors
      color1 = activeLightingConfig->frameColor1;
      color2 = activeLightingConfig->frameColor2;
      r = lerp((color1 >> 16) & 0xFF, (color2 >> 16) & 0xFF, t);
      g = lerp((color1 >> 8) & 0xFF, (color2 >> 8) & 0xFF, t);
      b = lerp(color1 & 0xFF, color2 & 0xFF, t);
      interpolatedColor = (r << 16) | (g << 8) | b;

      
      status = LP5024_SetColor(LED6, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED5, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED4, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED3, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED2, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED1, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;

      // Interpolate between the two button colors
      color1 = activeLightingConfig->buttonColor1;
      color2 = activeLightingConfig->buttonColor2;
      r = lerp((color1 >> 16) & 0xFF, (color2 >> 16) & 0xFF, t);
      g = lerp((color1 >> 8) & 0xFF, (color2 >> 8) & 0xFF, t);
      b = lerp(color1 & 0xFF, color2 & 0xFF, t);
      interpolatedColor = (r << 16) | (g << 8) | b;

      // Set LED color
      status = LP5024_SetColor(LED7, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED0, Adjust_Color_Brightness(interpolatedColor, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      
      // Update t based on the transition direction
      speed = SPEED_MIN + (SPEED_MAX - SPEED_MIN) * activeLightingConfig->patternSpeed / 100.0f;
      t += speed * transitionDirection; // Adjust this value to control the speed and direction of the transition

      // Check if t reaches the limits and handle accordingly
      if (t > 1.0f) {
        t = 1.0f; // Set t to 1.0f for a smooth transition without blink
        transitionDirection = -1; // Reverse the transition direction
      } else if (t < 0.0f) {
        t = -t; // Calculate the absolute value of t
        transitionDirection = 1; // Reverse the transition direction
      }
      break;

    case PATTERN_RAINBOW_CYCLE:
      // Calculate the hue increment based on pattern speed
      hueIncrement = (activeLightingConfig->patternSpeed / 100.0f) * HUE_STEP;

      // Convert HSV to RGB
      HSVtoRGB(hue, 1.0, 1.0, &r, &g, &b); // Assuming full saturation and value

      // Set LED color
      status = LP5024_SetColor(LED7, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED6, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED5, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED4, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED3, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED2, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED1, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      status = LP5024_SetColor(LED0, Adjust_Color_Brightness((r << 16) | (g << 8) | b, activeLightingConfig->brightness));
      if(status != HAL_OK) return status;
      // Update hue
      hue = fmodf((hue + hueIncrement), HUE_MAX);
      
      break;
      
    default:
      // Unknown report type
      break;
  }
  return status;
}
