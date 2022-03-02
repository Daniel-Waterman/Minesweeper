# Minesweeper

This contains my replica of the classic game that I was first introduced to through Windows XP.
I have created this to be as faithful to the orignal as possible, including graphics, audio and settings,
using C#/.NET 6 and WPF to create the user interface.

#### Try it yourself
If you wish to try it yourself, download the 'Ready-to-Run' folder and open MinesweeperApp.exe.

### In-Action Screenshots

**Game start in beginner mode**

![Beginner Game at start](/Images/EasyScreenshot.png)

**Menu settings**

![Menu](/Images/MenuScreenshot.png)

**During intermediate game**

![Game in progress](/Images/InGameScreenshot.png)

**Game won with best times dialog**

![Game won with best times dialog](/Images/WinScreenshot.png)

**Game lost, all unflagged mines revealed**

![Game lost, all mines revealed](/Images/DeathScreenshot.png)


### Details
- The project is broken down into the logic library (MinesweeperLogic) and front-end windows app (MinesweeperApp).
- The backend library contains code for the game logic that would be the same no matter how the GUI is implemented.
  I have written this so that any implementation should be possible with this code (terminal, web etc.).
- The front-end UI uses WPF. I have tried to use an MVVM design for the interaction between front-end and logic.
  It is not a complete MVVM design as there is some limited code in the code behind of the views, otherwise it would
  have meant far more complicated code for no real benefit.
- Some interesting parts (at least for me!) are:
  - Use of events to trigger timing and end of game actions.
  - Implementation of recursion to reveal grid squares.
- All graphics were recreated by me.


### Issues/Future Versions
- This implementation does not guarantee that the board is solvable *without* guessing. Some versions of the
  game calculate the grid in such a way that a solution without guessing is guaranteed. I have been unable to 
  find out if the original windows version implemented this. In the future, I may implement a guaranteed solve version.
- **Future:** A web app version, which would rely on the same back-end logic and a new front-end.
