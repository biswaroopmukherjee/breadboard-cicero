Breadboard-cicero
============
Save experimental parameters written by [Cicero](http://akeshet.github.io/Cicero-Word-Generator/).

## How does it work?

Reads runlog files. Writes snippet csv files as well as JSON run objects to a web API.
## How can I use it?

Ask me for prebuilt binaries or clone the repo and build it yourself (see below). This program must be run on the Cicero control computer.

Run `breadboard.exe`. You'll see:

![startup](https://raw.githubusercontent.com/biswaroopmukherjee/breadboard-cicero/master/docs/breadboard-cicero-1-empty.PNG)

Fill in the settings. For the API token and URL, ask me. We usually store the snippet files in the server. The runlog files are near your Cicero installation.

![settings](https://raw.githubusercontent.com/biswaroopmukherjee/breadboard-cicero/master/docs/breadboard-cicero-2-settings.png)

Hit start. The program will watch the current runlog folder for updates.

![start](https://raw.githubusercontent.com/biswaroopmukherjee/breadboard-cicero/master/docs/breadboard-cicero-3-start.png)

Every time Cicero runs a sequence, it writes a runlog, and breadboard-cicero writes a snippet file as well as a run object.

![newrun](https://raw.githubusercontent.com/biswaroopmukherjee/breadboard-cicero/master/docs/breadboard-cicero-4-newrun.png)

## How can I build it?

Open the solution in Visual studio (2017 or higher). Build for x86. Use the files saved in `Breadboard\bin\x86\Release`.

## FAQs:

1. Can I run this without an internet connection?
A: Yes but your snippet files will be saved locally.

2. How do I read the snippet files?
Use the matlab code [here](https://github.com/bec1/Data-Explorer-GUI/tree/master/Snippet-Functions), or the python code [here]
