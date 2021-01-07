set projectName=Compufunk_Triangle

call git config --add lfs.customtransfer.lfs-folder.path lfs-folderstore
call git config --add lfs.customtransfer.lfs-folder.args "D:/Dropbox/10_Git_LFS\/%projectName%/LFS"
call git config --add lfs.standalonetransferagent lfs-folder

call git push origin master