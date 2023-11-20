import cv2
import numpy as np
import argparse

def main(args):
    im = cv2.imread(args.rgb)
    with open(args.txt, 'r') as f:
        labels = f.readlines()


    c = [[ann.split(' ')] for ann in labels]
    c = [[[float(k) for k in ann] for ann in i] for i in c]

    #for det in c:
    #    print('det', det)

    c = [[[np.reshape(np.array(det[1:]), (-1,2))] for det in lista] for lista in c]
    print(len(c))
    print(len(c[0]))
    print(len(c[0][0]))
    print(len(c[0][0][0]))
    print(len(c[0][0][0][0]))

    c = np.squeeze(np.squeeze(c, 1), 1)
    #print(c)
    # c = [[[int(c[i][k][0]*im.shape[0]), int(c[i][k]*im.shape[1])] for k in len(det)] for i in len(c)]
    c = [[[int(pair[0]*im.shape[1]), int(pair[1]*im.shape[0])] for pair in det] for det in c]

    # print(c[0][1])
    # print(c[0])
    mask_true = cv2.drawContours(im, [np.array(c[0])], 0, (255,0,0), 3 )
    # print(mask_true)
    #cv2.imwrite('/home/elena/repos/yolov7/annotation2mask/paper8.jpg', mask_true)
    cv2.imshow('check annotations', mask_true)
    cv2.waitKey(0)

if __name__ == "__main__":
    # Create a parser object
	parser = argparse.ArgumentParser(description="Description of your script")

    # Add command-line arguments
	parser.add_argument("--rgb", type=str, help="rgb filename path", default='/home/elena/repos/Cifarelli/results_07_09_23/images/image_03.png')
	parser.add_argument("--txt", type=str, help="txt filename path", default='/home/elena/repos/Cifarelli/results_07_09_23/images/image_03.txt')

    # Parse the command-line arguments
	args = parser.parse_args()

    # Call the main function with the parsed arguments
	main(args)







