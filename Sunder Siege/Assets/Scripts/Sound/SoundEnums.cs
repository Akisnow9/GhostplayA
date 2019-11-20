

// This is the enums for all sound.
// Prepare your butts, this gets unnecisarily convoluted very quickly.
// Seriously I dunno why I bothered.



// The 'mixergroups'.
// Used mainly for volume control in this instance but you could probably
// have various effect(eg: reverb, delay) attached to groups and change them
// based on situation. Eg: all diagetic sounds get changed to a group with 
// large amounts of 'reverb' if the player falls into water. 
public enum Mixer
{
    None,
    DiageticMusic,
    NonDiageticMusic,
    DiageticSfx,
    NonDiageticSfx,
};


// The soundbank identifiers.
// These are used in the soundmanager so that when the 'soundrequester' component
// tried to find the sound it has something to look for. It also allows you to 'layer'
// sounds on top of each other. 

public enum SoundToPlay
{
    None,
    Burning,
    DoorSmash,
    Crumble,
    Falling,
    AbientSwordFight,
    WaterSplash,
    MenuMusic,
    LevelMusic,
    WinMusic,
    FailMusic,
    Soldier,
    Apple
};

// The 'Triggers'.
// These serve 2 purposes.
// At 'start'(not awake) everything that has a variable of soundrequester will 'request' a 
// new instance of the sound in the soundbank and attach it to 


public enum WhenToPlaySound
{
    None,
    Swing,
    Walk,
    Run,
    Drop,
    Clatter,
    Pending,
    Active,
    Fail,
    Success,
    Menu,
    LevelStart,
    LoseGame,
    Test,
    Fix,
    Fall
};