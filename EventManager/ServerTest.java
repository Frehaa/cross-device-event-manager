import java.net.*;
import java.io.*;

public class ServerTest {

  public static void main(String[] args) throws UnknownHostException, IOException, InterruptedException {
    ServerSocket serverSocket = new ServerSocket(10450);

    Socket socket = serverSocket.accept();  
    PrintWriter writer = new PrintWriter(socket.getOutputStream());
    BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));

    System.out.println("New connection");

    String line = reader.readLine();
    System.out.println("Received: " + line);
    writer.write(line);
    writer.flush();
    socket.close();
  }
}
