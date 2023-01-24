#!/bin/bash

# Set the starting and ending numbers
start=0
end=301

# Iterate through the numbered files
for ((i=start; i<=end; i++))
do
    # Construct the file name
    filename="input_$i.spt"

    # Check if the file exists
    if [ -f $filename ]
    then
        # Perform actions on the file
        rhash --simple $filename | cut -f1 -d ' '
    else
        # File does not exist
        echo "$filename does not exist"
    fi
done
