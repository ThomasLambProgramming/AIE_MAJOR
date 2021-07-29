Readme for making the prototype level without needing to ask me 999 questions for my aweful
implementation

CAMERA
Click and drag the cinemachine camera and the main camera into the scene (they have
some settings changed and the player will vibrate horribly if you dont just place them in
and try to do it yourself)


WIRE SYSTEM
click and drag the dummyWireModel in
click and drag the wirehackableobject in
drag the camera offset child from dummy wire model to wire hackable object on the hackable
object class and put it on the camera follow variable
adjust the trigger collider as needed and etc

to make the path just add to the wire script path variable as needed the path goes from top down
and will display a sphere to show the where it is in world


LEVER OBJECT
just place in world, it will rotate the opposite way it is currently facing, for the door working with
it just click and drag the door object into the onhacked event and go down to door and select the Door.Open
function and it will do its thing (there is also a close)


DOOR 
as said previously you just need a unity event system to click and drag it in with the door.open/close function
(btw the door just has an offset on where u want it to move you can use it as a bridge and whatnot as well)

TRIGGER VOLUME
click and drag the trigger volume in and adjust the collider as needed, fill in the unity events needed
thats it


MOVEABLE OBJECTS

click and drag the hackable object object adjust colliders and box as needed 
if you want it to be moveable on object type select enemy or moveable object (at the moment they do the same
thing)
if you change the size of the object just remember to move the camera offset as well or it wont be correct
and will look weird


i might have gotten stuff wrong but its all roughly right if any of it is wrong and its working properly just go into my
sceen and see how the objects are linked (dont touch it or there will be git issues)