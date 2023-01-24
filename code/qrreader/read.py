import cv2
from pyzbar import pyzbar
from zbar import Config, Image, Symbol
import zbar
import os
import io
import os    
from chardet import detect

# specify the named directory
qr_code_dir = '../../data/tmp'

# check if the directory exists or create it
if not os.path.exists(qr_code_dir):
    os.mkdir(qr_code_dir)
else:
    # clean up the directory
    for file in os.listdir(qr_code_dir):
        file_path = os.path.join(qr_code_dir, file)
        os.unlink(file_path)

# Initialize webcam
cap = cv2.VideoCapture(0)
# cap.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('M', 'J', 'P', 'G'))
width = 1280
height = 720
cap.set(cv2.CAP_PROP_FRAME_WIDTH, width)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, height)
# Initialize previous data
prev_data = None

# Initialize file index
file_index = 0

black_count = 0
scanner = zbar.ImageScanner()
scanner.parse_config('enable')
scanner.set_config(Symbol.QRCODE, Config.ENABLE, 1)
scanner.set_config(Symbol.QRCODE, Config.BINARY, 1)



while True:
    # Capture frame-by-frame
    ret, frame = cap.read()
    frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    height, width = frame.shape[:2]
    # Get pixel value of the top left corner
    pixel = frame[0,0]
    # check if the pixel is close to black
    if (True):
        # print(f"top left corner is close to black {black_count}")
        black_count += 1

        raw = frame.tobytes()
        image = zbar.Image(width, height, 'Y800', raw)

        scanner.scan(image)
        
         # Process QR codes
        for symbol in image:

            print('decoded', symbol.type)
            
            # Get QR code data
            
            data = bytes()
            data = symbol.data 
            # Compare data to previous data
            # if symbol.data != prev_data and len(symbol.data) > 10:
                # Save data to binary file with incremented file index
            file_name = f'input_{file_index}.spt'
            with open(os.path.join(qr_code_dir, file_name), 'wb') as f:
                f.write(data)
            # prev_data = symbol.data
            file_index += 1
            print(f"QR code data saved to {file_name}")

            # Draw rectangles around QR codes
            # (x, y, w, h) = qr_code.rect
            # cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 0, 255), 2)
    else:
        black_count = 0
    # Display the resulting frame
    cv2.imshow('QR code reader', frame)

    # Exit when Q is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release the webcam
cap.release()
cv2.destroyAllWindows()
