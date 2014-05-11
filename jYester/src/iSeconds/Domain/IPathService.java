package iSeconds.Domain;

public interface IPathService {
	
	String getApplicationPath();
    String getMediaPath();
    String getDbPath();
    String getFFMpegDbPath();
	String getCompilationPath();
	boolean isGood();
}
