package com.adminapp.ServerConnectionInstallation;
import com.adminapp.Lecture;

import org.springframework.http.ResponseEntity;

import com.adminapp.AnnotationList;
import com.adminapp.Lecture;

@AnnotationList.AbstractFeature(abstractFatureName = "UseServer")
@AnnotationList.Feature(featureName = "Server")
@AnnotationList.NeededAnyway(featureName = "Server")
public class ServerSetting {

    private static IServerConnection server = null;

    public static void init(boolean serverFeatureActivated) {
//        server = FeatureInstances.serverFeature.isFeatureActivated()
        server = serverFeatureActivated
                 ? new ServerConnection()
                 : new NoServerConnection();
    }

    public static ResponseEntity<String> postLecture(Lecture lecture) {
        ResponseEntity<String> postResponse = server.postLecture(lecture);
        return postResponse;
    }

    public static Lecture postThanGetLecture(Lecture lecture) {
        Lecture lect = server.postThanGetLecture(lecture);
        return lect;
    }

    public static void putLecture(Lecture lecture, long position) {
        server.putLecture(lecture, position);
    }

    public static Lecture getLectureByPosition(long position) {
        Lecture lecture = server.getLectureByPosition(position);
        return lecture;
    }
}