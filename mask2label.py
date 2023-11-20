'''This script provide a dataset for Yolov7-segmentation, starting from a Unity generated dataset with masks and rgb images.
The starting dataset could be generated by SimpleSpawnerEfficientPose.cs '''
'''Made by Elena Govi '''

import cv2
import numpy as np
import glob
import os
import argparse

def main(args):
    # Your main functionality goes here
	dataset_name = args.dataset_name
	label = str(args.label)
	path = args.dataset_path
	mask_filenames = glob.glob(args.masks) 
	rgb_filenames = glob.glob(args.rgbs) 
	vis = args.print
	dir_path = os.path.join(path+dataset_name)
	if vis:
		print('dir_path', dir_path)
	n_train_perc = args.train_perc
	n_val_perc = args.val_perc
	if vis:
		print('mask_number', len(mask_filenames))
		print('rgb_number', len(rgb_filenames))
	assert len(mask_filenames) == len(rgb_filenames)
	n_train = int(len(rgb_filenames)*n_train_perc/100)	
	n_val = int(len(rgb_filenames)*n_val_perc/100)
	n_test = len(rgb_filenames)-n_train-n_val
	if vis:
		print('n_train', n_train)
		print('n_val', n_val)
		print('n_test', n_test)

	try: 
		os.mkdir(dir_path) 
	except: 
		print('This directory has already been created. New data are added to the older ones.')

	train_dir = os.path.join(dir_path, 'train')
	try: 
		os.mkdir(train_dir) 
	except: 
		print('This directory has already been created. New data are added to the older ones.')

	val_dir = os.path.join(dir_path, 'val')
	try: 
		os.mkdir(val_dir) 
	except: 
		print('This directory has already been created. New data are added to the older ones.')

	test_dir = os.path.join(dir_path, 'test')
	try: 
		os.mkdir(test_dir) 
	except: 
		print('This directory has already been created. New data are added to the older ones.')

	print('Saving txt files with annotated masks ...')
	for k, filename in enumerate(mask_filenames):
		img = cv2.imread(filename)
		img_name = label+filename[-8:-4] #+'b'
		print('-------------------------------------------------------------')
		print('img_name', img_name)
		print('mask name', filename[-8:])
		img_grey = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
		width, height = img_grey.shape
		contours, hierarchy = cv2.findContours(img_grey, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
		annotations={}
		for i,count in enumerate(contours):
			annotations[i]=[]
			for c in count:
				annotations[i].append(c[0][0]/height)
				annotations[i].append(c[0][1]/width)
		if k<n_train:
			print('k in training', k)
			with open(os.path.join(train_dir, img_name)+'.txt','w') as f:
				if vis:
					print('txt filename', os.path.join(dir_path, img_name)+'.txt')
				for key in annotations.keys():
					# print('key', key)
					# print('len of annotations', len(annotations[key]))
					annotations_string = str(annotations[key]).replace(', ', ' ').replace('[', '').replace(']', '')
					f.write(label+' '+annotations_string+'\n')
		elif k>n_train-1 and k<n_train+n_val:
			print('k in val', k)
			with open(os.path.join(val_dir, img_name)+'.txt','w') as f:
				print('txt filename', os.path.join(dir_path, img_name)+'.txt')
				for key in annotations.keys():
					# print('key', key)
					# print('len of annotations', len(annotations[key]))
					annotations_string = str(annotations[key]).replace(', ', ' ').replace('[', '').replace(']', '')
					f.write(label+' '+annotations_string+'\n')
		else:
			print('k in test', k)
			with open(os.path.join(test_dir, img_name)+'.txt','w') as f:
				print('txt filename', os.path.join(dir_path, img_name)+'.txt')
				for key in annotations.keys():
					annotations_string = str(annotations[key]).replace(', ', ' ').replace('[', '').replace(']', '')
					f.write(label+' '+annotations_string+'\n')


	print('Saving rgb files to the correct directory ...')
	new_train_filenames = []
	new_val_filenames = []
	new_test_filenames = []
	for j, filename in enumerate(rgb_filenames):
		img = cv2.imread(filename)
		img_name = label+filename[-8:-4] #
		if j<n_train:
			if vis:
				print('rgb filename train', os.path.join(train_dir, img_name)+'.png')
			cv2.imwrite(os.path.join(train_dir, img_name)+'.png', img)
			new_train_filenames.append(os.path.join(train_dir, img_name)+'.png')
		elif j>n_train-1 and j<n_train+n_val:
			# print('rgb filename test', os.path.join(test_dir, img_name)+'.png')
			cv2.imwrite(os.path.join(val_dir, img_name)+'.png', img)
			new_val_filenames.append(os.path.join(val_dir, img_name)+'.png')

		else:
			# print('rgb filename test', os.path.join(test_dir, img_name)+'.png')
			cv2.imwrite(os.path.join(test_dir, img_name)+'.png', img)
			new_test_filenames.append(os.path.join(test_dir, img_name)+'.png')

	print('Saving files.txt with paths...')

	try:
		with open(dir_path+'/'+'train.txt', 'a') as f:
			for line in new_train_filenames:
				f.write(line+'\n')
	except:
		with open(dir_path+'/'+'train.txt', 'w') as f:
			for line in new_train_filenames:
				f.write(line+'\n')
	try:
		with open(dir_path+'/'+'val.txt', 'a') as f:
			for line in new_val_filenames:
				f.write(line+'\n')
	except:
		with open(dir_path+'/'+'val.txt', 'w') as f:
			for line in new_val_filenames:
				f.write(line+'\n')
	try:
		with open(dir_path+'/'+'test.txt', 'a') as f:
			for line in new_test_filenames:
				f.write(line+'\n')
	except:
		with open(dir_path+'/'+'test.txt', 'w') as f:
			for line in new_test_filenames:
				f.write(line+'\n')

	if vis:
		print('Done!')


	
		

if __name__ == "__main__":
    # Create a parser object
	parser = argparse.ArgumentParser(description="Description of your script")

    # Add command-line arguments
	parser.add_argument("--masks", type=str, help="mask filenames path", default="/home/elena/repos/Cifarelli/DataGeneration/Assets/Dataset/root1/data/01/mask/*.png")
	parser.add_argument("--rgbs", type=str, help="rgb filenames path", default='/home/elena/repos/Cifarelli/DataGeneration/Assets/Dataset/root1/data/01/rgb/*.png')
	parser.add_argument("--label", type=int, help='label of the objects', default = '0')
	parser.add_argument("--dataset_path", type=str, help='where do you want to save your dataset', default='/home/elena/Documents/')
	parser.add_argument("--dataset_name", type=str, help='decide the name of the new dataset', default='dataset_prova')
	parser.add_argument("--train_perc", type=int, default=70, help="Percentage of images for training")
	parser.add_argument("--val_perc", type=int, default=20, help="Percentage of images for validation")
	parser.add_argument('--print', help='print the phases', default = True)


    # You can add more arguments as needed
    # parser.add_argument("--optional_arg", type=int, default=42, help="Description of an optional argument")

    # Parse the command-line arguments
	args = parser.parse_args()

    # Call the main function with the parsed arguments
	main(args)


