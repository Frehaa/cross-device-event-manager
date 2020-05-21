import java.net.*;
import java.io.*;
import org.json.*;

public class ServerTest {

  public static void main(String[] args) throws UnknownHostException, IOException, InterruptedException {
    ServerSocket serverSocket = new ServerSocket(10451);

    Socket socket = serverSocket.accept();  
    DataOutputStream writer = new DataOutputStream(socket.getOutputStream());
    DataInputStream reader = new DataInputStream(socket.getInputStream());

    System.out.println("New connection");

    String line = reader.readUTF();

    JSONObject msg = new JSONObject(line);
    String eventName = msg.getString("name");
    JSONArray eventArgs = msg.getJSONArray("args");

    int arg0 = eventArgs.getInt(0);
    boolean arg1 = eventArgs.getBoolean(1);
    String arg2 = eventArgs.getString(2);
    double arg3 = eventArgs.getDouble(3);

    System.out.println("Event name: " + eventName);
    System.out.println("args[0]: " + arg0);
    System.out.println("args[1]: " + arg1);
    System.out.println("args[2]: " + arg2);
    System.out.println("args[3]: " + arg3);
    writer.writeUTF(line);
    writer.flush();
    socket.close();
  }
}
