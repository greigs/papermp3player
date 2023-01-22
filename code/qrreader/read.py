import cv2
from pyzbar import pyzbar
import os

# specify the named directory
qr_code_dir = '../data/tmp'

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

# Initialize previous data
prev_data = None

# Initialize file index
file_index = 0

while True:
    # Capture frame-by-frame
    ret, frame = cap.read()

    # Find QR codes in the frame
    qr_codes = pyzbar.decode(frame)

    # Process QR codes
    for qr_code in qr_codes:
        # Get QR code data
        data = qr_code.data

        # Compare data to previous data
        if data != prev_data:
            # Save data to binary file with incremented file index
            file_name = f'input_{file_index}.spt'
            with open(os.path.join(qr_code_dir, file_name), 'wb') as f:
                f.write(data)
            prev_data = data
            file_index += 1
            print(f"QR code data saved to {file_name}")


        # Draw rectangles around QR codes
        (x, y, w, h) = qr_code.rect
        cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 0, 255), 2)

    # Display the resulting frame
    cv2.imshow('QR code reader', frame)

    # Exit when Q is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release the webcam
cap.release()
cv2.destroyAllWindows()
