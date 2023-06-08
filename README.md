# ChroMapper Beat Saber Open Replay Viewer (CM-BSOR)

A [ChroMapper](https://github.com/Caeden117/ChroMapper) plugin designed for opening and viewing BeatLeader's [BS Open Replay format](https://github.com/BeatLeader/BS-Open-Replay).

## Installation

To install CM-BSOR, download the latest version of the plugin from [Releases](https://github.com/Caeden117/CM-BSOR/releases) and download the `CM-BSOR.dll` file to the `Plugins` subfolder of your ChroMapper installation.

## Notes

- The scale of notes in ChroMapper is **not** the same as the scale of notes in Beat Saber. Exact saber positions and note cuts will not be accurate compared to viewing a replay in Beat Saber.
- As of June 8th 2023, CM-BSOR works on both the latest **Stable** and **Development** channels of ChroMapper.
- CM-BSOR only extracts the metadata and keyframe portion of the replay. Information on notes, walls, height, and pauses are not extracted and viewable with this plugin.
- CM-BSOR has no map validation checks; even if the replay is for a different map entirely, the plugin will gladly load and view the replay.