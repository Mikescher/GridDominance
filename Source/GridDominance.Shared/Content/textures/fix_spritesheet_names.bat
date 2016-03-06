IF EXIST spritesheet.json (
    DEL  spritesheet-sheet.json
    MOVE spritesheet.json spritesheet-sheet.json

    DEL  spritesheet.json
)