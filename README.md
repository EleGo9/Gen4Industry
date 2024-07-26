## Gen4Industry
Data Generation Pipeline with Unity and Python Scripts for YOLOv7 Segmentation, YOLOv4 Detection and 6D pose estimation format.
Paper accepted in Computers in Industry: https://authors.elsevier.com/sd/article/S0166-3615(24)00058-7

# Open the project in Unity

1) download UNITY, I used versinon 2021.3.12f1.

2) download the github repo with ```git clone```

3) open the repo on Unity using 'add --> add project from disk' and selecting the folder 'Data Generation' from the repo. 

# Start with Data Customization

On the left (Hierarchy) you should have the following:
- Main Camera (on):
    - DirectionalLight(on), for custom random lights;
    - Spawner(on), where you can detail which object types (in 'Objects'), how many (in 'Amount') and their ID labels (in 'IDs');
    - SpawnerAreaUp(on), minimum distance from the camera;
    - SpawnerAreDown(on), maximum distance from the camera;
    - BasicPlane(on), plane where the object is positioned;
    - BasePlane (off), another kind of background, not used but available;
    - BinaryCamera(on), masks for segmentation labels;
    - DepthCamera(off), depth is not available.
- Caustic Volume(on)
- EventSystem(on)
- PostProcessing(on)
- CustomPlane(on)
    -geometrywallMetallic(on), for creating the semi-cluttered geometric background. Put off if you do not want it. 

The objects that can be added to the spawner are in the folder Prefabs. Is it also possible to insert your own objects, in the same format.


# Start with the Data Generation

Once you decided which kind of dataset you want, in the code 'Spawner' select the 'target number', i.e. the number of images to be generated.

Press pause, then press play, then press pause.

To stop the process, press again play. The dataset is saved in the 'Dataset' folder in the 'Assets'.

You should have generated the 'i-th' folder with the following structure:
- ds i
    - data
        - 01
            - mask
            - rgb
            - gt.yml
            - info.yml
    - models
        - model_info.yml


# Convert to 
# - YOLOv4 format (analogous to other YOLOvn detection formats)
In the folder dsi/data/01/rgb/ you can find images and their respective bounding box labels in a file.txt with the same name of the image. The label is as follows: " 
# - YOLOv7 segmentation format
Generate the conda environment from yolov7 repository
```conda create -n myenv python=3.9```
```conda install --file requirements.txt```

To generate a yolov7-seg dataset, run the following code:
```python mask2label.py```
where you must add the following input:
- ```--masks path/to/masks/images``` indicates the path to the masks folder, be sure to add also the masks format to the path, for example ```path/to/folder/*.png```;
- ```--rgbs path/to/rgbs/images``` indicates the path to the rgbs folder, be sure to add also the images format to the path, for example ```path/to/the/folder/*.png```;
- ```--label n``` where n is the label of the objects in the image. Remember that you can process one label for each object and that this code is available only for single-instance images. If you have more than one label, you should run this code multiple times, one for each label;
- ```dataset_path path/to/the/new/dataset``` where do you want to save your dataset?
- ```dataset_name new_name``` which is the name of your new dataset?
- ```train_perc m``` where m is the percentage of images saved for training;
- ```val_perc s``` where s is the percentage of images saved for validation. Note that test images will be the images remained after the percentages of train and val are taken; 
- ```--print True```


To check if the dataset has been correctly generated, use the following code:
```python annotations2mask.py --rgb /path/to/the/image/filname.png --txt /path/to/the/label/filename.txt```


# - 6D pose dataset for evaluation


