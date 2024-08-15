# BFI VRCFT Module 🧠

A VRCFT module that let's you control VRCFT avatars using BFI expression actions controlled by your brain 🧠

## How to use 🤔

### 1 - Make sure you have configured your supported expressions based on your trained actions :

`expressions.json` must be in the same folder as the module and is used to define which expressions you have trained and which action number it's assigned to.

Here's an exemple of a `expressions.json` supporting all of the supported expressions thus far:

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
    }
}
```

>List of supported expressions not set in stone yet

[Here's BFI's documentation](https://github.com/ChilloutCharles/BrainFlowsIntoVRChat/wiki/Action-Classification-Instructions) on how to train your own actions

### 2 - Launch BFI to output to port 8999 💨

here's the command to launch BFI with the right launch option:

`python .\main.py ----osc-port 8999`

>Using an OSC mixer is recomended to remain compatibility with your BFI parameters compatible avatar

### 3 - Drag and drop the latest release of the module in your VRCFT CustomLibs folder

Make sure that the name of the folder containing `BFI_VRCFT_Module.dll` & `module.json` & `expressions.json` is named `91a90618-b020-4064-8832-809b2ca2b3be`

### 4 - Launch VRCFT ✔️

Enjoy
