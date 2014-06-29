package iSeconds.Domain;

public interface IPathService {
	
	String getApplicationPath();
    String getMediaPath();
    String getDbPath();
    String getLegacyDbPath();
    String getFFMpegDbPath();
	String getCompilationPath();
	boolean isGood();
	boolean isLegacyDb();
	void turnLegacyDbDisabled();
}
