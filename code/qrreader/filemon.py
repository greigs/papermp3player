import os
import shutil
import pyinotify
import time
import zlib

class FileSizeHandler(pyinotify.ProcessEvent):
    written_count = 0
    partfile = '/home/greig/papermp3player/data/tmp/part.spt'
    tempfile = '/home/greig/papermp3player/data/tmp/temp.spt'
    

    def process_IN_MODIFY(self, event):
        file_path = event.pathname
        file_name = os.path.basename(file_path)
        if file_name == "temp.spt":
            try:
                file_size = os.path.getsize(self.tempfile)
                if file_size > 0:
                    time.sleep(0.1) # allow time for file to be fully written
                    if file_size % 2954 == 0: # verify size.. must be fullty writtern before trying to read
                        with open(self.tempfile, 'rb') as file:
                            file.seek(file_size - 2954)
                            data = file.read(2953)
                        checksum = hex(zlib.crc32(data))
                        print('read ', checksum)
                        try:
                            with open(self.partfile, 'rb') as input_file:                                
                                input_data = input_file.read(2953)
                            if data != input_data:
                                with open(self.partfile, 'wb') as copy_file:
                                    copy_file.write(data)
                                shutil.copyfile(self.partfile, "/home/greig/papermp3player/data/tmp/input_{}.spt".format(self.written_count))
                                self.written_count += 1
                                print('written file')
                        except FileNotFoundError:
                            with open(self.partfile, 'wb') as copy_file:
                                copy_file.write(data)
                            shutil.copyfile(self.partfile, "/home/greig/papermp3player/data/tmp/input_{}.spt".format(self.written_count))
                            self.written_count += 1
                            print('written file (in exception)')
                    else:
                        print('unexpected file size ',file_size)
                elif file_size > 0:
                    print(file_size)
            except FileNotFoundError:
                print('filenotfound when reading size')

folder_path = '/home/greig/papermp3player/data/tmp/'
event_handler = FileSizeHandler()
wm = pyinotify.WatchManager()
notifier = pyinotify.Notifier(wm, event_handler)
wm.add_watch(folder_path, pyinotify.ALL_EVENTS, rec=True)
notifier.loop()
