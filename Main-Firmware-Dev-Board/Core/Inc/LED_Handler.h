#include <stdbool.h>
#include "main.h"

// Defines the various lighting patterns for LEDs
typedef enum 
{     
    PATTERN_NONE,
    PATTERN_STATIC,
    PATTERN_BLINK,
    PATTERN_BLINK_BETWEEN,
    PATTERN_EASE_IN,
    PATTERN_EASE_BETWEEN,
    PATTERN_RAINBOW_CYCLE,
} LightingPattern;

// Defines the various types of reports that can be used to update LED settings
typedef enum {
    REPORT_PATTERN         = 0x00,
    REPORT_PATTERN_SPEED   = 0x01,
    REPORT_BRIGHTNESS      = 0x02,
    REPORT_FRAME_COLOR1    = 0x03,
    REPORT_FRAME_COLOR2    = 0x04,
    REPORT_BUTTON_COLOR1   = 0x05,
    REPORT_BUTTON_COLOR2   = 0x06,
} ReportLightEnum;

typedef struct
{
    bool settingChanged; //Boolean to track is any of these settings got updated
    LightingPattern pattern; //enum LED pattern
    uint8_t brightness; //LED Brightness 0 to 100
    uint8_t patternSpeed; //Pattern Speed 0 to 100
    uint32_t frameColor1; //Color: 0x000000 to 0xFFFFFF
    uint32_t frameColor2; //Color: 0x000000 to 0xFFFFFF
    uint32_t buttonColor1; //Color: 0x000000 to 0xFFFFFF
    uint32_t buttonColor2; //Color: 0x000000 to 0xFFFFFF
} LightingConfiguration;

extern LightingConfiguration idleLightingConfig;
extern LightingConfiguration buttonPressLightingConfig;
extern LightingConfiguration lidLiftLightingConfig;

void updateIdleLightingConfig(LightingPattern pattern, uint8_t brightness, uint8_t patternSpeed, uint32_t frameColor1, uint32_t frameColor2, uint32_t buttonColor1, uint32_t buttonColor2);
void updateButtonPressLightingConfig(LightingPattern pattern, uint8_t brightness, uint8_t patternSpeed, uint32_t frameColor1, uint32_t frameColor2, uint32_t buttonColor1, uint32_t buttonColor2);
void UpdateLightingConfiguration(LightingConfiguration* config, ReportLightEnum report, uint32_t value);
void updateBulkLightSettings(uint8_t *report_buffer, uint16_t buffer_length);
void LightingHandler(void);
