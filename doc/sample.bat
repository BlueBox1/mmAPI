@echo off

cd ../bin/x64/Release

IF [%1]==[] (SET PATH1=rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov) ELSE (SET PATH1=%1)

GOTO begin
:begin

START app_decode_client_sample_1a.exe url=stream:%PATH1% x=200 y=200 w=640 h=480 -repeat=1 -borders=1

GOTO end
:end

cd ../../../doc