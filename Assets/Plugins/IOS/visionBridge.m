#import "visionBridge.h"
#import "UnityFramework/UnityFramework-Swift.h"
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h> // for UIImage
#include <ImageIO/ImageIO.h>
#import <MobileCoreServices/MobileCoreServices.h> 

//Get the latest recognized text (already works)

const char* getLatestRecognizedText(void)
{

    NSString *text = [[VisionOCRPlugin shared] getLatestText];
    
    return text ? strdup([text UTF8String]) : strdup("");
}


void recognizeTextFromImage(CGImageRef image, int roiX, 
                            int roiY, int roiW, int roiH )
{
    [[VisionOCRPlugin shared] recognizeText:image roiX:roiX
                              roiY:roiY roiW:roiW
                              roiH:roiH];
}

 
//ISSUE:
/*
    BASED ON THIS CODE, IT WILL GENERATE A NEW IMAGE FOR APPLE AND WILL CREATE AN IMAGE FROM UNITY.
    ERROR: ASSUMING BASED ON UNITY ISSUE, THE DATA BYTES OF THE IMAGE ISN'T SUCCESFULLY TRANSFERED
    OVER CAUSING JUNK VALUES/IMAGE TO BE SUCESFULLY PROCESSED. 
    FIXED -- THIS WASN"T THE ISSUE, JUST SETTING UP THE UNITY XCPUIMAGE

*/
void recognizeTextFromBytes(const uint8_t* imageData, int width, int height, 
                            int roiX, int roiY, int roiW, int roiH)
{
    //basic error checking, if either variables single 0, 
    //immediate return.
    if (!imageData || width <= 0 || height <= 0  || roiW <= 0 || roiH <= 0 ) return;

    // Each pixel is 4 bytes, we do math computation.
    size_t bytesPerPixel = 4;
    size_t bytesPerRow = bytesPerPixel * width;
    size_t dataSize = bytesPerRow * height;
    int flippedRoiY = height - roiY - roiH;

    CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
    //checks to see if color space fails.
    if(!colorSpace) 
        return;
    //wrap buffer 
    CGDataProviderRef provider = CGDataProviderCreateWithData(
        NULL, imageData, dataSize, NULL);
    //checks to see if provider fails.
    if (!provider) {
        CGColorSpaceRelease(colorSpace);
        return;
    }
    //Create the full image with the given image cropped.
    CGImageRef fullImage = CGImageCreate(
        width,
        height,
        8,                       // bits per component
        32,                      // bits per pixel
        bytesPerRow,
        colorSpace,
        kCGBitmapByteOrder32Big | kCGImageAlphaPremultipliedLast,
        provider,
        NULL,
        false,
        kCGRenderingIntentDefault
    );
    //DEBUGGING
    //printf("ImageData: %p, Width=%d, Height=%d, ROI=(%d,%d,%d,%d)\n",
    //   imageData, width, height, roiX, roiY, roiW, roiH);

    if (fullImage)
    {
        //crop the ROI image
        CGRect roi = CGRectMake(roiX, roiY, roiW, roiH);
        roi = CGRectIntersection(CGRectMake(0, 0, width, height), roi);
        CGImageRef cropped = NULL;
        
         
        cropped = CGImageCreateWithImageInRect(fullImage, roi);
        
        //if it returns back of the cropped image, then 
        //process.
        if (cropped) {

            recognizeTextFromImage(cropped, roiX, roiY, roiW, roiH);
            CGImageRelease(cropped);
        }

        CGImageRelease(fullImage);
    }
 

    CGColorSpaceRelease(colorSpace);
    CGDataProviderRelease(provider);
}
