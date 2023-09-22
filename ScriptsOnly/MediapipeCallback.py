import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import cv2
from pythonosc import osc_message_builder
from pythonosc import udp_client
import time
from mediapipe import solutions
from mediapipe.framework.formats import landmark_pb2
import numpy as np
import matplotlib.pyplot as plt


cap = cv2.VideoCapture(0)

if not cap.isOpened():
    print("Failed to open the webcam")
    exit()


def send_osc_data(data_array, osc_address, osc_port):
    client = udp_client.SimpleUDPClient(osc_address, osc_port)
    for item in data_array:
        osc_message = osc_message_builder.OscMessageBuilder(address='/data')
        osc_message.add_arg(item[0])   # Add the first element of the tuple
        osc_message.add_arg(item[1])   # Add the second element of the tuple
        client.send(osc_message.build())

osc_address = '127.0.0.1'   # OSC server IP address
osc_port = 9000


def plot_face_blendshapes_bar_graph(face_blendshapes):
  # Extract the face blendshapes category names and scores.
  face_blendshapes_names = [face_blendshapes_category.category_name for face_blendshapes_category in face_blendshapes]
  face_blendshapes_scores = [face_blendshapes_category.score for face_blendshapes_category in face_blendshapes]
  # The blendshapes are ordered in decreasing score value.
  face_blendshapes_ranks = range(len(face_blendshapes_names))

  fig, ax = plt.subplots(figsize=(12, 12))
  bar = ax.barh(face_blendshapes_ranks, face_blendshapes_scores, label=[str(x) for x in face_blendshapes_ranks])
  ax.set_yticks(face_blendshapes_ranks, face_blendshapes_names)
  ax.invert_yaxis()

  # Label each bar with values
  for score, patch in zip(face_blendshapes_scores, bar.patches):
    plt.text(patch.get_x() + patch.get_width(), patch.get_y(), f"{score:.4f}", va="top")

  ax.set_xlabel('Score')
  ax.set_title("Face Blendshapes")
  plt.tight_layout()

# STEP 2: Create an FaceLandmarker object. 
base_options = python.BaseOptions(model_asset_path='G:\desktop\Mediapipe\landmarker.task')
options = vision.FaceLandmarkerOptions(base_options=base_options,
                                       output_face_blendshapes=True,
                                       output_facial_transformation_matrixes=True,
                                       num_faces=1)
detector = vision.FaceLandmarker.create_from_options(options)




while True:
    # Read the latest frame from the webcam
    ret, frame = cap.read()

    if not ret:
        print("Failed to read the frame")
        break
    # Convert the frame to a numpy array
    frame_np = np.array(frame)

    # Display the frame
    cv2.imshow('Webcam', frame)
    #timestamp_ms = cap.get(cv2.CAP_PROP_POS_MSEC)
    image = mp.Image(image_format=mp.ImageFormat.SRGB, data=frame)

    # STEP 4: Detect face landmarks from the input image.
    detection_result = detector.detect(image)
    #plot_face_blendshapes_bar_graph(detection_result.face_blendshapes[0])
    # STEP 5: Process the detection result. In this case, visualize it.
    try:
        detection_result.face_blendshapes[0].pop(0)
        index_score_pairs =  [(item.category_name, round(item.score, 4)*100) for item in detection_result.face_blendshapes[0]]
        index_score_pairs_string = " ".join(map(str,index_score_pairs))
        #print (index_score_pairs_string)
        send_osc_data(index_score_pairs, osc_address, osc_port)  

    except:
        print("FAILED TO TRACK")
        continue
    # Exit the loop if 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    time.sleep(0.03)


cap.release()
cv2.destroyAllWindows()
