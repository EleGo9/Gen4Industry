## Gen4Industry
Data Generation Pipeline with Unity and Python Scripts for YOLOv7 Segmentation, YOLOv4 Detection and 6D pose estimation format.

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


