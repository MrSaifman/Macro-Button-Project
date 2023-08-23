// #ifndef BUTTON_HANDLER_H
// #define BUTTON_HANDLER_H

#include "main.h"

#define REPORT_SIZE  1
#define BUTTON_PRESSED  1
#define BUTTON_HELD  2
#define BUTTON_RELEASED 3
#define LONG_PRESS_TIME  1000 // 1 second for a long press

// Initialize button related resources
void Button_Init(void);

// Button interrupt service routine to be called in EXTI interrupt handler
void Button_ISR(void);

// Periodically checks button status. Could be called from main loop if necessary.
void CheckButtonHoldStatus(void);

// #endif // BUTTON_HANDLER_H