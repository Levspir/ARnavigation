
# AR Navigation
Indoor navigation using Unity and Vuforia SDK
This project is in development. | Senior project for Bachelor of Science (Information Technology).
<br>
## Description Overall
This project are navigation system on android. Point your mobile's camera to building for find marker images taht resamble with image in database. When found, it will show AR that contains data of that position (if not in navigate mode).
User can known thier position and rotation direction from Top-view map. and can start navigate mode by choosing destination of that building from Search page. When in navigation mode and point your camera to marker, It will show AR red arrow point to next node. And will show AR green checked when reach destination.

## Abstract from my paper (may be easier to understand)
– Augmented reality based indoor navigation system running on a smartphone is proposed to be used for in-building navigation. The system uses a built-in camera to capture the image of surroundings, detects a natural marker in the image, and calculates the pose of the camera with respect to the marker. The position and orientation of the camera (which are the same as the smartphone itself) with respect to the indoor map are then determined using the pose information of that marker—note that each marker must be pre-registered with pose information in the system. Once the destination is specified by the user, the shortest path to that destination will be calculated and the arrow pointing along the path to the destination will be augmented on the scene. The information message explaining the route will also be annotated on the screen and be read out to help guide users to the destination. In addition, the system can display a top view map of building showing current position and facing direction of the user, and drawing the route to the destination—the top-view mode makes a better understanding and experience for the user. 

## ScreenShot
on start app[1] and found marker[2] marker will show like [3] in Top-view <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav04board.jpg" alt="on start app" width="150"/> <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav05board.jpg" alt="search page" width="150"/>  <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav08top.jpg" alt="search page" width="150"/>
<br>
Search page (I Search for room 317) <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav06search.jpg" alt="search page" width="150"/> <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav07search.jpg" alt="search page when type something" width="150"/>
<br>
before navigate will have dialog of destination's data and navigate button <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav11beforechoose.png" alt="first room" width="150"/>
<br>
in navigate mode (purple) <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav01arrow.jpg" alt="first room" width="150"/> <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav02arrow.jpg" alt="second room" width="150"/>
<br>
Top-view map (are relate below) <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav09top.jpg" alt="second room" width="150"/> <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav10top.jpg" alt="top view second room" width="150"/>
<br>
when reached destination <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/arnav03tick.jpg" alt="reached" width="150"/>
<br>
we can also navigate complex map using dijkstra's algorithm (this is my test map) <br>
<img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/complex1.png" alt="reached" width="150"/> <img src="https://raw.githubusercontent.com/fasterac/AR-Indoor-Navigation-System/master/ReadmeComponents/complex2.png" alt="reached" width="150"/>


------------------------------------------
