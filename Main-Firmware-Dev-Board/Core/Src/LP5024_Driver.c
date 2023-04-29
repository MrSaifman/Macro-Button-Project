#include <stdbool.h>
#include "main.h"
#include "stm32g0xx_hal_i2c.h"
#include "LP5024_Driver.h"

extern I2C_HandleTypeDef hi2c1;

HAL_StatusTypeDef LP5024_Init(void)
{
  HAL_StatusTypeDef status;
  
  //Enable the LP5024 IC 
  uint8_t chip_en[] = {0x00, 0x40};
  status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, chip_en, sizeof(chip_en), HAL_MAX_DELAY);

  //Enable auto increment feature
  uint8_t auto_inc_en[] = {0x01, 0x08};
  status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, auto_inc_en, sizeof(auto_inc_en), HAL_MAX_DELAY);

  uint8_t i2cCmd3[] = {0x02, 0x00};
  status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, i2cCmd3, sizeof(i2cCmd3), HAL_MAX_DELAY);
}

void LP5024_SetColor(enum LED_Color_Reg led_register, int hex_color)
{
    HAL_StatusTypeDef status;

    int red = (hex_color >> 16) & 0xFF;
    int green = (hex_color >> 8) & 0xFF;
    int blue = hex_color & 0xFF;
    
    uint8_t i2cCmd[] = {led_register, red, green, blue};
    status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, i2cCmd, sizeof(i2cCmd), HAL_MAX_DELAY);
}

void LP5024_SetBrightness(enum LED_Brightness_Reg led_register, int brightness)
{
    HAL_StatusTypeDef status;

    int hex_value = (brightness * 255) / 100;

    uint8_t i2cCmd1[] = {led_register, hex_value};
    status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, i2cCmd1, sizeof(i2cCmd1), HAL_MAX_DELAY);
}

void LP5024_GlobalOff(bool off)
{
    HAL_StatusTypeDef status;
    uint8_t data;

    if(off)
        data = LP5024_GLOBALOFF_BIT;
    else
        data = 0x00;
    
    uint8_t i2cCmd1[] = {LP5024_DEVICE_CONFIG1, data};
    status = HAL_I2C_Master_Transmit(&hi2c1, LP5024_I2C_ADDR_BCST << 1, i2cCmd1, sizeof(i2cCmd1), HAL_MAX_DELAY);
}

uint32_t Adjust_Color_Brightness(uint32_t color, uint8_t brightness_level) {
    // Extract the individual color components (red, green, and blue)
    uint8_t red = (color >> 16) & 0xFF;
    uint8_t green = (color >> 8) & 0xFF;
    uint8_t blue = color & 0xFF;

    // Scale the color components based on the brightness level
    red = (red * brightness_level) / 255;
    green = (green * brightness_level) / 255;
    blue = (blue * brightness_level) / 255;

    // Combine the modified color components back into a single value
    return (red << 16) | (green << 8) | blue;
}

enum LED_Color_Reg LedNum_To_ColorReg(uint8_t led_index) {
    switch (led_index) {
        case 0:
            return LED0;
        case 1:
            return LED1;
        case 2:
            return LED2;
        case 3:
            return LED3;
        case 4:
            return LED4;
        case 5:
            return LED5;
        case 6:
            return LED6;
        case 7:
            return LED7;
        default:
            return LEDERR;
    }
}