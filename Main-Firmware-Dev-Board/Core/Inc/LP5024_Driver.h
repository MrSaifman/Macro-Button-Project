#define LP5024_I2C_ADDR_BCST 0x3C //Broadcast Address
#define LP5024_I2C_ADDR0 0x28 // ADDR0 = GND & ADDR1 = GND
#define LP5024_I2C_ADDR1 0x29 // ADDR0 = GND & ADDR1 = VCC
#define LP5024_I2C_ADDR2 0x2A // ADDR0 = VCC & ADDR1 = GND
#define LP5024_I2C_ADDR3 0x2B // ADDR0 = VCC & ADDR1 = VCC

#define LP5024_DEVICE_CONFIG1  0x01
#define LP5024_GLOBALOFF_BIT  0x01

enum OutRegister {
    OUT0 = 0x0F,
    OUT1 = 0x10,
    OUT2 = 0x11,
    OUT3 = 0x12,
    OUT4 = 0x13,
    OUT5 = 0x14,
    OUT6 = 0x15,
    OUT7 = 0x16,
    OUT8 = 0x17,
    OUT9 = 0x18,
    OUT10 = 0x19,
    OUT11 = 0x1A,
    OUT12 = 0x1B,
    OUT13 = 0x1C,
    OUT14 = 0x1D,
    OUT15 = 0x1E,
    OUT16 = 0x1F,
    OUT17 = 0x20,
    OUT18 = 0x21,
    OUT19 = 0x22,
    OUT20 = 0x23,
    OUT21 = 0x24,
    OUT22 = 0x25,
    OUT23 = 0x26,
};

enum LED_Color_Reg {
    LED0 = 0x0F,
    LED1 = 0x12,
    LED2 = 0x15,
    LED3 = 0x18,
    LED4 = 0x1B,
    LED5 = 0x1E,
    LED6 = 0x21,
    LED7 = 0x24,
};

enum LED_Brightness_Reg {
    LED0_BRIGHTNESS = 0x07,
    LED1_BRIGHTNESS = 0x08,
    LED2_BRIGHTNESS = 0x09,
    LED3_BRIGHTNESS = 0x0A,
    LED4_BRIGHTNESS = 0x0B,
    LED5_BRIGHTNESS = 0x0C,
    LED6_BRIGHTNESS = 0x0D,
    LED7_BRIGHTNESS = 0x0E,
};

void LP5024_Init(void);
void LP5024_SetColor(enum LED_Color_Reg led_register, int hex_color);
void LP5024_SetBrightness(enum LED_Brightness_Reg led_register, int brightness);
void LP5024_GlobalOff(bool off);