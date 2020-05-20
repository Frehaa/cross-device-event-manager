import java.net.*;
import java.io.*;

public class SocketTest {

  public static void main(String[] args) throws UnknownHostException, IOException, InterruptedException {
    String address = args[0];
    int port = Integer.parseInt(args[1]);

    Socket socket = new Socket(address, port);

    BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
    PrintWriter writer = new PrintWriter(socket.getOutputStream());

    System.out.println("Sending...");
    writer.write("Hello over there\n");
    writer.flush();

    String line = in.readLine();
    System.out.println(line);

    writer.close();
    socket.close();
  }

}
