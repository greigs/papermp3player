ffmpeg -rtbufsize 1k -i pipe: -f wav pipe:1 | ffplay -nodisp -
