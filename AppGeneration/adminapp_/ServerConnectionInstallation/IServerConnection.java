package com.adminapp.ServerConnectionInstallation;
import com.adminapp.Lecture;
import com.adminapp.AnnotationList;

import org.springframework.http.ResponseEntity;

@AnnotationList.ConnectedToFeature(featureName = "Server")
public interface IServerConnection {
    ResponseEntity<String> postLecture(Lecture lecture);
    Lecture postThanGetLecture(Lecture lecture);
    Lecture getLectureByPosition(long position);
    void putLecture(Lecture lecture, long position);
}
