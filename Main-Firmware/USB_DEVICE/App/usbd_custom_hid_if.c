/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : usbd_custom_hid_if.c
  * @version        : v2.0_Cube
  * @brief          : USB Device Custom HID interface file.
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2023 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Includes ------------------------------------------------------------------*/
#include "usbd_custom_hid_if.h"

/* USER CODE BEGIN INCLUDE */
#include "LED_Handler.h"
#include "usb_device.h"
/* USER CODE END INCLUDE */

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/

/* USER CODE BEGIN PV */
/* Private variables ---------------------------------------------------------*/
extern uint8_t report_buffer[64]; 
extern bool settingsLoaded;
#define REPORT_NONE            0x00
#define REPORT_IDLE_LIGHT      0x01
#define REPORT_BUTTON_LIGHT    0x02
#define REPORT_LID_LIFT_LIGHT  0x03
#define REPORT_BUTTON_DURATION 0x04
#define BULK_SETTINGS_LOAD     0x05
/* USER CODE END PV */

/** @addtogroup STM32_USB_OTG_DEVICE_LIBRARY
  * @brief Usb device.
  * @{
  */

/** @addtogroup USBD_CUSTOM_HID
  * @{
  */

/** @defgroup USBD_CUSTOM_HID_Private_TypesDefinitions USBD_CUSTOM_HID_Private_TypesDefinitions
  * @brief Private types.
  * @{
  */

/* USER CODE BEGIN PRIVATE_TYPES */

/* USER CODE END PRIVATE_TYPES */

/**
  * @}
  */

/** @defgroup USBD_CUSTOM_HID_Private_Defines USBD_CUSTOM_HID_Private_Defines
  * @brief Private defines.
  * @{
  */

/* USER CODE BEGIN PRIVATE_DEFINES */

/* USER CODE END PRIVATE_DEFINES */

/**
  * @}
  */

/** @defgroup USBD_CUSTOM_HID_Private_Macros USBD_CUSTOM_HID_Private_Macros
  * @brief Private macros.
  * @{
  */

/* USER CODE BEGIN PRIVATE_MACRO */
/* USER CODE END PRIVATE_MACRO */

/**
  * @}
  */

/** @defgroup USBD_CUSTOM_HID_Private_Variables USBD_CUSTOM_HID_Private_Variables
  * @brief Private variables.
  * @{
  */

/** Usb HID report descriptor. */
__ALIGN_BEGIN static uint8_t CUSTOM_HID_ReportDesc_FS[USBD_CUSTOM_HID_REPORT_DESC_SIZE] __ALIGN_END =
{
  /* USER CODE BEGIN 0 */

  // Usage Page (Generic Desktop Controls)
    0x05, 0x01,        
    // Usage (Vendor-Defined)
    0x09, 0x00,        
    // Start Collection (Application)
    0xA1, 0x01,        
    
    // Button
    0x05, 0x09,        // Usage Page (Button)
    0x19, 0x01,        // Usage Minimum (Button 1)
    0x29, 0x01,        // Usage Maximum (Button 1)
    0x15, 0x00,        // Logical Minimum (0)
    0x25, 0x01,        // Logical Maximum (1)
    0x95, 0x01,        // Report Count (1)
    0x75, 0x01,        // Report Size (1 bit)
    0x81, 0x02,        // Input (Data, Var, Abs)
    
    // Two Hall Effect Sensors
    0x05, 0x09,        // Usage Page (Button)
    0x19, 0x02,        // Usage Minimum (Button 2)
    0x29, 0x03,        // Usage Maximum (Button 3)
    0x95, 0x02,        // Report Count (2)
    0x75, 0x01,        // Report Size (1 bit)
    0x81, 0x02,        // Input (Data, Var, Abs)

    // Padding to 1 byte for the above three inputs
    0x95, 0x05,        // Report Count (5)
    0x81, 0x03,        // Input (Constant) - 5 bits padding

    // LP5024 LED Controller Commands
    // Assuming command length is 8 bytes for simplicity
    0x06, 0x00, 0xFF,  // Usage Page (Vendor-Defined)
    0x09, 0x01,        // Usage (Vendor-Defined)
    0x95, 0x08,        // Report Count (8)
    0x75, 0x08,        // Report Size (8 bits = 1 byte)
    0x15, 0x00,        // Logical Minimum (0)
    0x26, 0xFF, 0x00,  // Logical Maximum (255)
    0x91, 0x02,        // Output (Data, Var, Abs)

  /* USER CODE END 0 */
  0xC0    /*     END_COLLECTION	             */
};

/* USER CODE BEGIN PRIVATE_VARIABLES */

/* USER CODE END PRIVATE_VARIABLES */

/**
  * @}
  */

/** @defgroup USBD_CUSTOM_HID_Exported_Variables USBD_CUSTOM_HID_Exported_Variables
  * @brief Public variables.
  * @{
  */
extern USBD_HandleTypeDef hUsbDeviceFS;

/* USER CODE BEGIN EXPORTED_VARIABLES */
void USB_Recieve_Task(uint8_t *report);
/* USER CODE END EXPORTED_VARIABLES */
/**
  * @}
  */

/** @defgroup USBD_CUSTOM_HID_Private_FunctionPrototypes USBD_CUSTOM_HID_Private_FunctionPrototypes
  * @brief Private functions declaration.
  * @{
  */

static int8_t CUSTOM_HID_Init_FS(void);
static int8_t CUSTOM_HID_DeInit_FS(void);
static int8_t CUSTOM_HID_OutEvent_FS(uint8_t *report, uint16_t len);

/**
  * @}
  */

USBD_CUSTOM_HID_ItfTypeDef USBD_CustomHID_fops_FS =
{
  CUSTOM_HID_ReportDesc_FS,
  CUSTOM_HID_Init_FS,
  CUSTOM_HID_DeInit_FS,
  CUSTOM_HID_OutEvent_FS
};

/** @defgroup USBD_CUSTOM_HID_Private_Functions USBD_CUSTOM_HID_Private_Functions
  * @brief Private functions.
  * @{
  */

/* Private functions ---------------------------------------------------------*/

/**
  * @brief  Initializes the CUSTOM HID media low layer
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_Init_FS(void)
{
  /* USER CODE BEGIN 4 */
  return (USBD_OK);
  /* USER CODE END 4 */
}

/**
  * @brief  DeInitializes the CUSTOM HID media low layer
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_DeInit_FS(void)
{
  /* USER CODE BEGIN 5 */
  return (USBD_OK);
  /* USER CODE END 5 */
}

/**
  * @brief  Manage the CUSTOM HID class events
  * @param  event_idx: Event index
  * @param  state: Event state
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_OutEvent_FS(uint8_t *report, uint16_t len)
{
  /* USER CODE BEGIN 6 */

  // At this point, USB_Receive_Buffer contains the received data.
  // You can process the data as needed.
  USB_Recieve_Task(report);

  return (USBD_OK);
  /* USER CODE END 6 */
}

/* USER CODE BEGIN 7 */

void USB_Recieve_Task(uint8_t *report) {
  //Check if the first byte of the report buffer equals 1
  if (report[0] == REPORT_IDLE_LIGHT) {
    uint32_t theSetting = (report[2] << 16) | (report[3] << 8) | report[4];
    UpdateLightingConfiguration(&idleLightingConfig, report[1], theSetting);
  } else if (report[0] == REPORT_BUTTON_LIGHT) {
    uint32_t theSetting = (report[2] << 16) | (report[3] << 8) | report[4];
    UpdateLightingConfiguration(&buttonPressLightingConfig, report[1], theSetting);
  } else if (report[0] == REPORT_LID_LIFT_LIGHT) {
    uint32_t theSetting = (report[2] << 16) | (report[3] << 8) | report[4];
    UpdateLightingConfiguration(&lidLiftLightingConfig, report[1], theSetting);
  } else if (report[0] == REPORT_BUTTON_DURATION) {
    uint32_t theSetting = (report[2] << 16) | (report[3] << 8) | report[4];
    UpdateButtonConfiguration(&buttonConfig, report[1], theSetting);
  } else if (report[0] == BULK_SETTINGS_LOAD) {
    updateBulkLightSettings(report, sizeof report);
    settingsLoaded = true;
  }
}

/**
  * @brief  Send the report to the Host
  * @param  report: The report to be sent
  * @param  len: The report length
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */

//static int8_t USBD_CUSTOM_HID_SendReport_FS(uint8_t *report, uint16_t len)
//{
//  return USBD_CUSTOM_HID_SendReport(&hUsbDeviceFS, report, len);
//}

/* USER CODE END 7 */

/* USER CODE BEGIN PRIVATE_FUNCTIONS_IMPLEMENTATION */

/* USER CODE END PRIVATE_FUNCTIONS_IMPLEMENTATION */
/**
  * @}
  */

/**
  * @}
  */

/**
  * @}
  */

