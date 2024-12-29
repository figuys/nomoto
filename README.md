# nomoto

## backstory

Motorola offers a recovery tool for their phones and other devices. This tool is called "Software Fix". The previous name was "Recovery and Smart Assistant" or RSA for simplicity. The tools was originally developed by Lenovo. It was called something by them, I don't know what they called it. I do, however, know it was referred to using the acronym LMSA.

The tool is available for download. You must have a Motorola/Lenovo account in order to download the tool. The tool takes the approach where it does anything and everything it can think to do - well... not anything, but you understand?.. Yes?

The tool has developed a reputation for being extremely capable of device recovery. It is also known as an extremely simple tool that anyone can use. The tool operates against the device in any mode. It talks basic MPT. It talks ADB. It talks Fastboot. It talks both Qualcomm and MediaTek. The tool is published and available in all of the global markets where Motorola/Lenovo sell and support devices.

I was trying to flash different images onto a Motorola device of mine in a effort to get the bootloader unlocked. I did bad... I brick phone...

### OH NOES

I installed RSA. I plugged my completely bricked phone to my computer using the instructions provided by the RSA tool. The tool proceeded to use highly specialized tools from Qualcomm to identify the device, match the device to an image file that the phone was meant to run, download the image to my computer, and finally install the image onto the phone bringing the device back to an ideal state. It even went so far as to recover my profile and all of my files.

This came as a complete surprise to me given that I knew exactly what I did in order to brick the phone in the first place. I managed to wipe some critical disk partitions, the kind of thing that is generally considered as "unrecoverable". i.e. boot, bootloader, vbmeta, vendor and other.... you guessed it... FRP.

Given all that I know about devices and all I have seen, I would have called it completely impossible to flash a phones TEE and other highly secure areas with the phone forcing a complete wipe of all user data. But that's not what happened. What happened was it fixed it and my shit still there.

DaFuq?

## and then

Right away it became quite obvious to me that this RSA tool has baked into it some very impressive secrets with respect to how to use low level communication to manipulate Qualcomm and MediaTek devices. Again, I was completely flabergasted because I had tried and tried and tried anything and everything I could think up or google up.

So I spent the next couple of weeks capturing USB traces between the computer and the phone whilst flashing stuff in fastboot, or during recoveries after intentionally bricking the device. I say that like I know what means, or how to do that. I sorta knew what it meant. I kind-of had an idea about how. But not really. No actual experience.

My experience is being a relatively boring corporate programmer. Sure... I write code. But the type of stuff I write and the languages that I know and the tools that I have experience with are childs play compared to this shit. Like I know what hexadecimal is... I know hwo to use the Windows calculator to do conversions. I have a vague idea about what a memory offset it and why that might matter. 

I did spend a couple of years in college... like 20 years ago... thinking that I was going to become the C++ greybeard master of the machine, where I would get rich because I made something special that was basically magic to anyone else. I figured that one day I would become a distinquished engineer and that... like I'd get written about in magazines because algorithms... 

Turns out there was no possible way that I could ever even grow a grey five-o-clock shadow, much less a serial killer inspired grey beard. I'm sure I was smart enough to get there, but I'm way to fucking lazy and so... not interested in all that work. I'm all about doing simple shit, that I act like is really hard, and stretching out two days worth of work into a three month contract. 

All this based entirely on the illusion that because I know more than most people, I'm actually one of the best. The best would laugh at me. 

But this fucking RSA application did it's thing and that motivated the shit out of me, because of that obvious power that could be had if one could figure out how the app is doing what it does. So like six weeks go by and I'm no closer than day one. In fact, I'm pretty sure, at that point, that I had actually made everything worse. 

fuck this typing shit... the code is in the repo. it's damn cool.