## ![alt text][logo] AttackOfTheDots ##
[logo]: https://avatars0.githubusercontent.com/u/11473656?s=50

**Attack Of The Dots** is a simple game where you control a slow-calibrating cannon against oncoming dots featuring 10 levels of increasing difficulty.

This repository contains a `Playable Build - PC` folder, feel free to clone the repository and try out the game for yourself.

### Gameplay ###

#### _Video_ ####

- Take a look at the gameplay video [here](https://www.youtube.com/watch?v=bpJpi7iBPA4 "Link to the Youtube video").

#### _Enemies_ ####

- **Yellow** is a slow dot. In the first couple of levels it is the only enemy type available.
- **Blue** is an average-paced dot. You'll notice it from level 3 onwards.
- **Green** is a fast dot. You'll notice it from level 5 onwards.
- **Brown** is an armoured dot, this means it will take two shots to kill it. He is slow and turns into a yellow dot after the first shot. You'll notice this dot from level 7 onwards.
- **Purple** is the rarest dot. It changes direction every second, making it tough to aim for. However it is very rare and you are only likely to see it in levels 9 and 10.

#### _Cannon_ ####

- The cannon is a slow-calibrating weapon, meaning that you have 3 seconds between clicking a target area and the explosion actually going off. This means you need to time your shots well, and especially in the higher levels, you need to prioritise which dots should die first.

### Project ###

#### _Code_ ####

- This project was made in Unity 5.3 in C#.
- This is released under the MIT License so feel free to clone the repository and open the `AttackOfTheDots` folder in Unity to look or edit.

#### _Animations_ ####

 - Due to this project being more of a demonstration than a game to be released, I used a few different animation methods:
 	- The first of course is Unity's Mecanim State system, which animates the Grin when you lose the level.
 	- The second is the camera animation which precedes the Grin appearing. This is done through the `Update()` method.
 	- The third is the menu animation. This relies on a position variable `menu_position` which is calculated in the `Update()` method and is displayed through the `OnGUI()` functionality.
 	- The fourth is a shading progression for the title screen. This consists of 3 images being drawn with varying alpha colours, allowing the system to display a fade-in-and-out effect.

#### _Art Assets_ ####

 - All the assets were made by myself in Paint & Paint.net so I apologise in advance for the pixelated horrors.
 - The explosion sprite is a royalty-free sprite I found.