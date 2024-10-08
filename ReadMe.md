# BFI VRCFT Module 🧠

A VRCFT module that let's you control VRCFT avatars using BFI expression actions controlled by your brain 🧠

[![Watch the video](media/BFI_VRCFT_thumbnail.png)](https://youtu.be/25exXjnTCI0)


## Supported expressions 😀

>List of supported expressions not set in stone yet

| Reference                                           | Expression          | Description                                               |
|:---:                                                | :-------------:     |    :-------------:                                        |
| <img src="media/eyeclosed.gif" alt="eyeclosed"/> | eyeclosed           |  eyelids closing fully                                    |
| <img src="media/smile.gif" alt="smile"/>         | smile               | smile with mouth opened                                   |
| <img src="media/frown.gif" alt="frown"/>         | frown               | n shaped frown                                            |
| <img src="media/anger.gif" alt="anger"/>         | anger               | brow going down                                           |
| <img src="media/cringe.gif" alt="cringe"/>       | cringe              | mouth stretches and reveles the bottom teeth              |
| <img src="media/cheekpuff.gif" alt="cheekpuff"/> | cheekpuff           | cheek puffs                                               |
| <img src="media/apeshape.gif" alt="apeshape"/>   | apeshape            | lower jaw while keeping the mouth closed                  |

## Tested hardware 🧰

While this module is compatible with [any Brainflows compatible boards](https://github.com/ChilloutCharles/BrainFlowsIntoVRChat#instructions), training facial expressions has only been tested with the MuseS and Muse 2 headbands thus far.

Please get in contact if you try this with a different device so I can list it here 😊

## How to use 🤔

### 1 - Train your expressions as BFI Actions🏋️ :

Make sure you've trained your actions

[Here's BFI's documentation](https://github.com/ChilloutCharles/BrainFlowsIntoVRChat/wiki/Action-Classification-Instructions) on how to train your own actions

### 2 - Get data into the module 💨

You've got a couple ways you can go about doing that but using an OSC mixer is recomended to remain compatibility with your BFI parameters compatible avatar.

#### OSC Mixing 🎛️

We'll use [VOR](https://github.com/SutekhVRC/VOR) but the setup should be about the same with any other OSC mixing software:

- Select a port for BrainFlows
![VORConfigBFI](media/VORConfigBFI.png)


- Configure your routing
![VORConfigVRCFT](media/VORConfigVRCFT.png)

- Launch BFI

  `python .\main.py --board-id=YOURBOARD --enable-action --osc-port=9002`



#### Directly output to the module ➡️

Here's the command to launch BFI with the right launch option if you leave the port on the default setting:

`python .\main.py ----osc-port 8999`


⚠️The port can be configured at the bottom of `config.json`

### 3 - Install the module 📁

Go to VRCFT's modules registery tab, click the `+` button and select the latest release of the module 📁

### 4 - Configured your supported expressions based on your trained actions 🤓

config.json must be in the same folder as the module and is used to define which expressions you have trained and which action number it's assigned to and it's overhall weight.

Here's an exemple of a config.json supporting all of the supported expressions thus far:


```
{
  "supportedexpressions": {
    "eyeclosed": {
      "id": 1,
      "weight": 1.0
    },
    "smile": {
      "id": 2,
      "weight": 1.0
    },
    "frown": {
      "id": 3,
      "weight": 1.0
    },
    "anger": {
      "id": 4,
      "weight": 1.0
    },
    "cringe": {
      "id": 5,
      "weight": 1.0
    },
    "cheekpuff": {
      "id": 6,
      "weight": 1.0
    },
    "apeshape": {
      "id": 7,
      "weight": 1.0
    }
  },
  "ip": "127.0.0.1",
  "port": 8999,
  "timouttime": 3.0
}
```

### 5 - Launch VRChat and switch to a VRCFT compatible avatar

VRCFT compatible avatars can be found by searching for `VRCFT` or `Face Tracing` in [Prismic's Avatar Search world](https://vrchat.com/home/world/wrld_57514404-7f4e-4aee-a50a-57f55d3084bf)

## Credits 📕

**[VRCFT](https://github.com/benaclejames/VRCFaceTracking)**

**[BrainFlowsIntoVRChat](https://github.com/ChilloutCharles/BrainFlowsIntoVRChat)**

**[VOR](https://github.com/SutekhVRC/VOR)**