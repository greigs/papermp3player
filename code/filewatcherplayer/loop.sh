while true
do

  SERVICE="ffplay"
  if pgrep -x "$SERVICE" >/dev/null
  then
    echo "$SERVICE is running"
    sleep 1
  else
    echo "$SERVICE stopped"
    dotnet run --no-build --project ~/papermp3player/code/filewatcherplayer
  fi
done
