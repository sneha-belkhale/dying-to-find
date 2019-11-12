cmd=$1
app_name="com.codercat.DyingToFind"

case $cmd in
  "init")
    shift
    # Install Oculus Integration tools
    git clone https://github.com/Kif11/oculus-integration Assets/Oculus
    ;;
  "remote")
    shift
    # Configure remote adb for a connected device
    port=5555
    adb tcpip $port
    sleep 3
    device_ip=$(adb shell ip addr show wlan0 | grep "inet\s" | awk '{print $2}' | awk -F'/' '{print $1}')
    adb connect ${device_ip}:${port}
    ;;
  "log")
    shift
    # Listen for Unity log from Android device
    adb logcat -s Unity ActivityManager PackageManager dalvikvm DEBUG
    ;;
  "clear")
    shift
    # Remove project from android device
    adb uninstall $app_name
    ;;
  "upload")
    shift
    ovr-platform-util upload-mobile-build --app_id 3023686394315929 --app_secret `cat app-secret` --apk Builds/MonsterWithin.apk --channel alpha --notes $1
    ;;
  *)
    echo "Usage: utils <cmd>"
    echo "Commands:"
    echo "  init - initilize project"
    echo "  remote - configure adb over wifi"
    echo "  log - show Unity logs from headset"
    echo "  clear - remove previosly installed app"
    echo "  upload <comment> - upload app to Oculus store"
esac