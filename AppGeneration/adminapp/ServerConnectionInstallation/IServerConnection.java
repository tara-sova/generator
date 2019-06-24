package com.adminapp.ServerConnectionInstallation;

import com.adminapp.Lecture;

import org.springframework.http.ResponseEntity;

public interface IServerConnection {
    ResponseEntity<String> postLecture(Lecture lecture);
    Lecture postThanGetLecture(Lecture lecture);
    Lecture getLectureByPosition(long position);
    void putLecture(Lecture lecture, long position);
}
