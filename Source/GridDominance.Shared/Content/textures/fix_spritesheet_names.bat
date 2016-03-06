IF EXIST spritesheet.json (
    IF EXIST spritesheet-sheet.json (
        DEL  spritesheet-sheet.json
        MOVE spritesheet.json spritesheet-sheet.json
    )

    DEL  spritesheet.json
)