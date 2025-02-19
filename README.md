# Unity Virtual Sessions Package

Unity package for creating virtual session games.

**Avaliable on Asset Store**: https://assetstore.unity.com/packages/slug/309268

**Forum thread**: https://discussions.unity.com/t/virtual-session-manager-open-source/1603167

## Overview
This Unity package provides a set of customizable prefabs and scripts designed to facilitate virtual sessions. It includes:

- **Customizable Prefabs**: Prebuilt objects with everything needed for virtual sessions.
- **Session Management Scripts**: Scripts to control visibility and interaction between sessions.
- **Demo Scene**: A simple scenario demonstrating the package's functionality.

## Installation
There are 3 different ways of adding the package to your project:
- import [VirtualSession.unitypackage](https://github.com/dsineirobarreiro/VirtualSession/releases/download/v1.0.0/VirtualSession.unitypackage) via _Assets-Import Package_
- clone/[download](https://github.com/dsineirobarreiro/VirtualSession/archive/refs/tags/v1.0.0.zip) this repository and move the root folder to your Unity project's Assets folder
- import it from [Asset Store](https://assetstore.unity.com/packages/slug/309268)

## Usage
### Prefabs
The package includes two basics prefabs for quick integration:

* **_NetworkPlayer.prefab_**: _PlayerObject_ with the basic functionalities necessary for the correct functioning of inter-session visibility. Open to its extension in the customisation of attributes or methods.
* **_Session.prefab_**: _NetworkObject_ in charge of managing visibility between clients connected to the same session.

To use these two prefabs, make sure that they are included in the _Network Prefab List_ managed by the _NetworkManager_, and select the _NetworkPlayer.prefab_ as the `Default Player Prefab` in the _NetworkManager_ configuration.
_Session.prefab_ is instantiated from the _SessionManager.cs_, so be sure to indicate the path where the prefab is located.

### Demo Scene
A sample scene is included to demonstrate a basic implementation. Open it via `VirtualSession/DemoScene` to explore.

## Scripts Documentation
### `SessionManager.cs`
**Description**: Handles the current active sessions.

**Methods**:
- `GameObject GetSession(string sessionId)`: Gets the _GameObject_ associated to the session.
- `void AssignToSession(string session, ulong clientId)`: Assigns the new connected client to the session. If it does not exist, it creates one by calling `CreateGameSession(session)`.
- `void CreateGameSession(string sessionId)`: Instantiates a session and spawns it. The new session is added to active sessions.
- `void RemoveGameSession(string sessionId)`: Removes the session from the actives sessions and destroys its _GameObject_.

### `Session.cs`
**Description**: Manages the logic inside a single session.

**Methods**:
- `void OnNetworkSpawn()`: Overrides default implementation when client-side code running. It requests the server to parent the client to the session's _GameObject_ and makes visible already connected clients.
-  `void ShowClientInSessionServerRpc(ulong clientObjectId, ulong clientIdToShow)`: _ServerRPC_ used to make visible a _NetworkObject_ to a single client.
- `void PlayersChanged(NetworkListEvent<ulong> changeEvent)`: Listens to changes on the players list and performs the corresponding actions. When a new client is added to the session, `ShowClientInSessionServerRpc(ulong clientObjectId, ulong clientIdToShow)` is called for every connected client.
- `void AddPlayer(ulong clientId)`: Called from _SessionManager.cs_, it makes visible the session to the new client and adds the client to the players list.
- `void ParentClientToSessionObjectServerRpc(ServerRpcParams serverRpcParams = default)`: _ServerRPC_ used for parenting the client to the session.

### `NetworkPlayer.cs`
**Description**: Manages the logic of a single client.

**Methods**:
- `void OnNetworkSpawn()`: Overrides default implementation for both client-side (only the owner) and server-side. If it is the owner, it asks the server to be assigned to the session. If it is the server, it subscribes `CheckVisibility` to the client's visibility delegate.
-  `bool CheckVisibility(ulong clientId)`: As they spawn with no observers, it is changed the `CheckObjectVisibility` delegate of the client to return true for the owner of the script, i.e. the client itself.
- `void UnsubscribeVisibility()`: Unsubscribes the visibility delegate to reset to default behaviour.
- `void AddPlayer(ulong clientId)`: Called from _SessionManager.cs_, it makes visible the session to the new client and it adds the client to the players list.
- `AssignToSessionServerRpc(string session, ServerRpcParams serverRpcParams = default)`: _ServerRPC_ called from `OnNetworkSpawn` to assign the client to the session. It calls `AssigntToSession` from _SessionManager.cs_.

## Contribution
Feel free to submit issues, feature requests, or pull requests to improve the package.

## License
This project is licensed under the MIT License. See `LICENSE` for details.

## Contact
For questions or support, reach out via the repository's issue tracker.

