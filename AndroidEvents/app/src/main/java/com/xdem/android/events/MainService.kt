package com.xdem.android.events

import android.app.Service
import android.content.Intent
import android.os.*
import android.os.Process.THREAD_PRIORITY_BACKGROUND
import android.util.Log
import android.widget.Toast
import java.io.BufferedReader
import java.io.InputStreamReader
import java.io.PrintWriter
import java.lang.StringBuilder
import java.net.HttpURLConnection
import java.net.InetAddress
import java.net.Socket
import java.net.URL

class MainService : Service() {
    private var networkThread: Thread = NetworkThread(this)


    private class NetworkThread(service : MainService) : Thread() {
        private val s : MainService = service
        private val port : Int = 10450

        override fun run() {
            val socket = Socket(InetAddress.getByName("192.168.1.3"), port)
            val reader = BufferedReader(InputStreamReader(socket.getInputStream()))
            val writer = PrintWriter(socket.getOutputStream())

            writer.println("Hello from Android")
            writer.flush()

            val line = reader.readLine()
            Log.i("NetworkThread", "Message from server " + line)
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