#include "buttonHandler.h"
#include "usbd_custom_hid_if.h"
#include "usb_device.h"

static uint8_t buttonStatus = BUTTON_RELEASED;

static uint32_t lastTick = 0;
static uint32_t startTick = 0;

extern USBD_HandleTypeDef hUsbDeviceFS;  // Ensure the USB handle is accessible

void Button_Init(void)
{
    // Add initialization code if necessary. For now, it's empty as GPIO is assumed to be initialized elsewhere.
}

void Button_ISR(void)
{
	if((HAL_GetTick() - lastTick) > 20)  // Debounce time of 20ms
	    {
	        lastTick = HAL_GetTick();
	    }
}

void CheckButtonHoldStatus(void)
{
    uint8_t report[REPORT_SIZE] = {0};

    // If button is pressed
    if (HAL_GPIO_ReadPin(GPIOC, GPIO_PIN_13) == GPIO_PIN_RESET) 
    {
        // If the previous state was RELEASED, then it's a fresh press
        if (buttonStatus == BUTTON_RELEASED) 
        {
            startTick = HAL_GetTick();
            buttonStatus = BUTTON_PRESSED;
            // Do not send the report here. We will wait for the release or for the long press time.
        }
        // If the button has been pressed for longer than LONG_PRESS_TIME
        else if ((HAL_GetTick() - startTick) >= LONG_PRESS_TIME && buttonStatus != BUTTON_HELD) 
        {
            buttonStatus = BUTTON_HELD;
            report[0] = BUTTON_HELD;  // This indicates a long press
            USBD_CUSTOM_HID_SendReport(&hUsbDeviceFS, report, REPORT_SIZE);
        }
    }
    // If button is released
    else 
    {
        if (buttonStatus == BUTTON_PRESSED) 
        {
            buttonStatus = BUTTON_RELEASED;
            report[0] = BUTTON_RELEASED; // This indicates a quick press and release
            USBD_CUSTOM_HID_SendReport(&hUsbDeviceFS, report, REPORT_SIZE);
        }
        // If button was previously in HELD state, just update the state to RELEASED. No report sent.
        else if(buttonStatus == BUTTON_HELD)
        {
            buttonStatus = BUTTON_RELEASED;
        }
    }
}
