#pragma once
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

const char* getLatestRecognizedText(void);
void recognizeTextFromImage(CGImageRef image, int roiX, int roiY, int roiW, int roiH);
void recognizeTextFromBytes(const uint8_t* imageData, int width, int height, 
                            int roiX, int roiY, int roiW, int roiH);

#ifdef __cplusplus
}
#endif
