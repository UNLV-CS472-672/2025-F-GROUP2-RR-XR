import Foundation
import Vision
import UIKit

@objc public class VisionOCRPlugin: NSObject {

    @objc public static let shared = VisionOCRPlugin()
    private var latestText: String = ""

    @objc public func getLatestText() -> String {
        return latestText
    }
    //let simpleDigits = try Regex("[0-9]+")
    @objc(recognizeText:roiX:roiY:roiW:roiH:)
    public func recognizeText(from image: CGImage,
                              roiX: CGFloat,
                              roiY: CGFloat,
                              roiW: CGFloat,
                              roiH: CGFloat) {
        // Create the Vision request
        let request = VNRecognizeTextRequest { [weak self] request, error in
            guard let self = self,
                  let observations = request.results as? [VNRecognizedTextObservation] else { return }

            self.latestText = observations
                .compactMap { $0.topCandidates(1).first?.string }
                .joined(separator: " ")

            //print("OCR Result: \(self.latestText)")
        }
        request.recognitionLevel = .accurate
        request.usesLanguageCorrection = true
        request.minimumTextHeight = 0.2
        //request.customWords = ["AEB" , simpleDigits]

        // Prepare cropped image if ROI is valid
        var targetImage: CGImage? = image
        let roi = CGRect(x: roiX, y: roiY, width: roiW, height: roiH)
        if roi.width > 0, roi.height > 0, let cropped = image.cropping(to: roi) {
            targetImage = cropped
        }

        // Run Vision on background queue
 
        DispatchQueue.global(qos: .userInitiated).async {

            if let cgImage = targetImage {
                let handler = VNImageRequestHandler(cgImage: cgImage, options: [:])
                
                do {
                    try handler.perform([request])
                } catch {
                    print("OCR failed")
                }
            }
        }
    }
}
