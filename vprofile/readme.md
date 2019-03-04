## volume profile

### requires 
### in mt4
- the mt4 indi to be live and same instruments to be loaded in both mq4 and alveo
- use mt4 script to create new vp box (vp interactively drawn in box when box location or size is update by user)
- file is output on changes

### in alveo indi
- and path to mq4's local output to be in the "path" section of alveo indi
- files are read (if path is correct) and drawn. All profiles are redrawn on file change (presumably from mt4)
