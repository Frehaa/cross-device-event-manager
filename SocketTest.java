import java.net.*;
import java.io.*;

public class SocketTest {

  public static void main(String[] args) throws UnknownHostException, IOException, InterruptedException {
    String address = args[0];
    int port = Integer.parseInt(args[1]);

    Socket socket = new Socket(address, port);

    DataInputStream in = new DataInputStream(socket.getInputStream());
    DataOutputStream writer = new DataOutputStream(socket.getOutputStream());

    System.out.println("Sending...");
    writer.writeUTF("Hello over there\n");
    writer.flush();

    String line = in.readUTF();
    System.out.println(line);

    writer.close();
    socket.close();
  }

}
