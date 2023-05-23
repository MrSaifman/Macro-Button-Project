#include <stdbool.h>
#include "main.h"
#include "LED_Handler.h"

// Definitions of idle and button press lighting configurations
LightingConfiguration idleLightingConfig;
LightingConfiguration buttonPressLightingConfig;

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
