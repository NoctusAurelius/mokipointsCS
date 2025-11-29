# Achievement Sound Effects

This folder contains sound effects for the achievement system.

## Folder Structure

```
Sounds/
└── Achievements/
    ├── unlock_common.mp3
    ├── unlock_uncommon.mp3
    ├── unlock_rare.mp3
    ├── unlock_epic.mp3
    ├── unlock_legendary.mp3
    └── unlock_mythical.mp3
```

## File Naming Convention

- Format: `unlock_[rarity].mp3`
- Rarity values: `common`, `uncommon`, `rare`, `epic`, `legendary`, `mythical`
- All lowercase

## Current Files (Verified)

✅ `unlock_common.mp3`  
✅ `unlock_uncommon.mp3`  
✅ `unlock_rare.mp3`  
✅ `unlock_epic.mp3`  
✅ `unlock_legendary.mp3`  
✅ `unlock_mythical.mp3`

## Sound File Requirements

- **Format**: MP3 (primary format for web compatibility)
- **Duration**: 0.5-2 seconds (short, non-intrusive)
- **Volume**: Normalized to similar levels across all files
- **Quality**: 128kbps or higher
- **File Size**: Keep under 100KB per file for fast loading
- **Sample Rate**: 44.1kHz recommended

## Usage

**Important**: Sound effects are played **ONLY** when the dashboard achievement notification popup appears. They are **NOT** played in:
- Profile page (when viewing achievements)
- Achievement gallery page
- Family chat (system messages)
- Any other location

Sound effects are triggered via JavaScript in the dashboard achievement notification popup, with different sounds for each rarity level. The system will fallback to `unlock_common.mp3` if a rarity-specific sound is not found.

## Implementation

Sounds are triggered via JavaScript in the dashboard achievement notification popup. See `PATCH_5.0.5_ACHIEVEMENT_SYSTEM.md` for implementation details.

---

## Copyright & Licensing

**Sound Effects Credits:**

The achievement unlock sound effects used in this project are sourced from Pixabay and are licensed under the Pixabay License, which allows free commercial and non-commercial use.

**Sound Effect Creators:**

1. **freesound_community** (Pixabay User: [46691455](https://pixabay.com/users/freesound_community-46691455/))
2. **universfield** (Pixabay User: [28281460](https://pixabay.com/users/universfield-28281460/))
3. **pwlpl** (Pixabay User: [16464651](https://pixabay.com/users/pwlpl-16464651/))

**License Information:**

- **License**: Pixabay License
- **Usage**: Free for commercial and non-commercial use
- **Attribution**: Not required, but appreciated
- **Modification**: Allowed
- **Source**: [Pixabay](https://pixabay.com/)

For more information about the Pixabay License, visit: https://pixabay.com/service/license/

---

**Note**: Sound effect files are already added to this folder following the naming convention above.

