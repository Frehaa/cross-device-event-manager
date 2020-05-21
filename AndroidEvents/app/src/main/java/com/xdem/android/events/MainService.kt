package com.xdem.android.events

import android.app.Service
import android.content.Intent
import android.os.*
import android.util.Log
import android.widget.Toast
import org.json.JSONArray
import org.json.JSONObject
import java.io.*
import java.net.InetAddress
import java.net.Socket

class MainService : Service() {
    private var networkThread: Thread = NetworkThread(this)


    private class NetworkThread(service : MainService) : Thread() {
        private val s : MainService = service
        private val port : Int = 10451

        override fun run() {
            val socket = Socket(InetAddress.getByName("192.168.1.3"), port)
            val reader = DataInputStream(socket.getInputStream())
            val writer = DataOutputStream(socket.getOutputStream())

            val message = JSONObject()
            message.put("name", "test_event")
            val args = JSONArray()
            args.put(5)
            args.put(true)
            args.put("String")
            args.put(0.2)
            message.put("args", args)

            Log.i("NetworkThread", message.toString())

            writer.writeUTF(message.toString())
            writer.flush()

            val line = reader.readUTF()
            Log.i("NetworkThread", "Message from server: " + line)
            socket.close()

            sleep(1000)
            s.stopSelf()
        }
    }

    override fun onBind(intent: Intent?): IBinder? = null

    override fun onCreate() {
        Toast.makeText(this, "Service created", Toast.LENGTH_LONG).show()

        networkThread.start()

        Log.i("MainService", networkThread.state.name)
    }



    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Toast.makeText(this, "Service started", Toast.LENGTH_LONG).show()
        Log.i("MainService", networkThread.state.name)

        return START_STICKY
    }

    override fun onDestroy() {
        Toast.makeText(this, "Service destroyed", Toast.LENGTH_LONG).show()
        Log.i("MainService", networkThread.state.name)
        networkThread.interrupt()
        Log.i("MainService", networkThread.state.name)
    }
}