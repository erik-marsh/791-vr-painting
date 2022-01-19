# Multiplayer VR Template

## Installation

1. Enable preview packages in Unity
2. Import the *OpenXR Plugin* and *XR Interaction Toolkit* packages
3. Under the *XR Interaction Toolkit* package, expand *Samples* and import the *Default Input Actions*
4. Go to Edit > Project Settings
5. Under *XR Plug-in Management*, check the box next to OpenXR
6. There should be a warning symbol that appears next to the OpenXR line. Click that and follow the instructions.
7. The networking libraries (**P**hoton **U**nity **N**etworking) should already be included in this project

## Project Setup

The ping pong scene should have all the necessary components for basic multiplayer VR. These components are:

### XR specific
* Input action manager (InputActionManager)
  * Action assets => XRI Default Input Actions (this is the thing you imported before)
* Interaction Managers (XRInteractionManager)
  * You may have multiple interaction managers in your scene to allow for you to semantically differentiate different actions that the player may use. For example, you could disable teleportation by disabling the teleportation interaction manager.
  * Pay attention to the names that you give these managers: there is an interaction manager hook script that requires you to provide a name for the interaction manager you want the interactor/interactable to link with.
    * This is required due to Photon requiring that room objects be instantiated at runtime. Ordinarily, you could just directly reference the manager in the scene, but this requires the interactable to be instantiated statically.
* Teleportation areas (if you plan to use teleportation) (TeleportationArea)
  * These can be made child objects of your floor; I think they will automatically stretch to encompass the extent of your floor. Be sure to set the teleportation area to sit a little bit higher than the floor (+0.001 units should work) so that it is visible to the teleportation provider.
* Hook scripts:
  * An InteractionManagerHook script must be placed on each interactor/interactable that you wish to place in the scene. This allows the interactable/interactor to assign themselves to an interaction manager at runtime.

### Photon specific
* Prefabs that you wish to use over the network must be placed in Assets/Resources.
* PhotonHandler
* All entities that must be visible over the network must have a PhotonView component. These entities must be instantiated at runtime with PhotonNetwork.Instantiate or PhotonNetwork.InstantiateRoomObject
  * Specializations of the PhotonView component may be added as necessary. For example, a networked rigidbody would require a PhotonRigidbodyView component in addition to the PhotonView component.
* NetworkInstantiation
  * This is a script that will automatically instantiate room objects (objects that have the same lifetime as the room). Place this on any objects/prefabs you wish to instantiate in the scene, and supply the script with the name of the prefab you wish to instantiate.
* TransferOwnership
  * This is a script that provides a function that transfers ownership of the object to the client who called the function.
  * This function is intended to be used with events. For example, add this function to the SelectEntered event of an XRGrabInteractable to ensure that the client that is holding the object also owns it. This makes physics smoother for the client that grabbed the object.

### General components
* Launcher (LaunchServerRoom)
  * Connects to and initializes the Photon server.
* PlayerManager
  * Utility object that provides a global reference to the player.
* RoomManager (Prefab)
  * Manages instantiation of the player.
* Spawnpoints (Prefab)
  * Make sure to set the indices in the order you want the spawnpoints to be used.

### The player prefab
There are two main components to the NetworkedPlayer prefab. The first is the parts that are present locally. These are typically the components that track the VR headset and controllers, manage input, etc. The second component is the networked representation of the player. These are components that are presented to the other players in the scene, such as the player’s head and hands.

A voice connection is instantiated when the player connects.

The Unity XR Interaction Toolkit only allows one controller component on an object. If you wish to have multiple interactors on one hand, you will have to use the ControllerManager component, and make all of your controllers for that hand child objects of the ControllerManager. You can then decide on an input that will cycle through the controllers.


## Photon Voice

The VoiceConnection prefab should have working settings (aside from the server IP, I would imagine you want to change that). The most important part of this prefab is the Transmit Enabled flag in the Recorder component of this prefab. When this flag is enabled, the user’s voice will be transmitted across the network. I have this flag unset by default. You can enable it straight up if you want always-on voice transmission (evil), or you can write a monobehaviour that gives you push-to-talk functionality.

Note: during my testing, the voice connection would throw the following error at least once:

`OnJoinRandomFailed errorCode=32760 errorMessage=No match found`

I believe that you can safely ignore this and voice will work fine. It seems to be attempting to join a room before it has fully connected to the voice server, and will eventually join a room properly.

## Other Remarks

I would recommend staying away from physics-heavy games/simulations. Photon does not use an authoritative physics server; the authority is the client who has ownership of the object. This means you have to do a lot of ownership transfer to ensure things look right to the right players. Even so, this can still be troublesome. The higher the speed of the objects being simulated, the harder it will be to work with the physics. The ping pong demo should be a good example of how things can get out of control very quickly.

This project uses OpenXR as its backend. This was chosen to maximize compatibility with VR devices. However, OpenXR is still a very new standard, and is missing some useful features. For example, support for Vive trackers was only added a few months ago (https://github.com/KhronosGroup/OpenXR-SDK/releases/tag/release-1.0.20), and these changes have not yet propagated to Unity’s OpenXR environment yet.
(This may have been fixed recently [on January 11, 2022, which was three days ago as of writing this!], but I have yet to test it. Related info: https://store.steampowered.com/news/app/250820?emclan=103582791435040972&emgid=3126061077819506317 
https://forum.unity.com/threads/openxr-and-openvr-together.1113136/ (this thread, toward the end))

## Example of an Interactable

* Rigidbody
* XRBaseInteractable
  * Such as an XRGrabInteractable
  * When the player interacts with this object, we want one of the interactable events to call `TransferOwnership.transferOwnership()`
    * Usually the select event
* TransferOwnership
* InteractionManagerHook
  * Attaches the object to the correct interaction manager (not required if you only use one interaction manager)
* PhotonView
* PhotonRigidbodyView
* PhotonTransformView
* NetworkInstantiation
  * Automatically handles instantiation of a room object when the server starts