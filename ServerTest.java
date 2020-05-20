import java.net.*;
import java.io.*;

public class ServerTest {

  public static void main(String[] args) throws UnknownHostException, IOException, InterruptedException {
    ServerSocket serverSocket = new ServerSocket(10451);

    Socket socket = serverSocket.accept();  
    DataOutputStream writer = new DataOutputStream(socket.getOutputStream());
    DataInputStream reader = new DataInputStream(socket.getInputStream());

    System.out.println("New connection");

    String line = reader.readUTF();
    System.out.println("Received: " + line);
    writer.writeUTF(line);
    writer.flush();
    socket.close();
  }
}
