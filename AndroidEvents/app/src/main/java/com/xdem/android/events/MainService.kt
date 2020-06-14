package com.xdem.android.events

import android.app.*
import android.content.Intent
import android.os.*
import android.util.Log
import android.widget.Toast
import org.json.JSONObject
import java.io.OutputStream
import java.lang.Process
import java.net.InetAddress
import java.net.Socket
import java.nio.charset.Charset
import java.util.*
import kotlin.concurrent.schedule
import kotlin.concurrent.thread
import kotlin.text.Charsets.UTF_8

class MainService : Service() {
    private val charset = UTF_8

    private val address: InetAddress = InetAddress.getByName("192.168.1.3")
    private val port: Int = 10451
    private val networkThread: NetworkThread = NetworkThread(address, port)
    private var networkHandler: NetworkHandler? = null

    private inner class NetworkThread(address: InetAddress, port: Int) : Thread() {
        private val address = address
        private val port = port
        private var socket: Socket? = null

        override fun run() {
            Log.i("Testing", "Preparing thread")
            Looper.prepare()
            socket = Socket(address, port)
            networkHandler = NetworkHandler(socket!!.getOutputStream())
            Log.i("Testing", "Looping")
            Looper.loop()
            Log.i("Testing", "End of loop")
        }
    }

    private inner class NetworkHandler(outputStream: OutputStream) : Handler() {
        private val writer: OutputStream = outputStream

        override fun handleMessage(msg: Message) {
            val jsonMessage = msg.obj as JSONObject
            writer.write(jsonMessage.toString().toByteArray(charset))
            writer.write("\n".toByteArray(charset))
            writer.flush()
        }
    }

    override fun onBind(intent: Intent?): IBinder? = null

    override fun onCreate() {
        Toast.makeText(this, "Service created", Toast.LENGTH_LONG).show()
/*
        val pendingIntent: PendingIntent = Intent(this, MainActivity::class.java).let {
            PendingIntent.getActivity(this, 0, it, 0)
        }

        val notification: Notification = Notification.Builder(this, "channelId")
            .setContentTitle("ContentTile")
            .setContentText("ContentText")
            .setSmallIcon(R.drawable.ic_launcher_background)
            .setContentIntent(pendingIntent)
            .setTicker("TickerText")
            .build()

        startForeground(1, notification)*/

        networkThread.start()

        val jsonObject = JSONObject()
        jsonObject.put("Event", "onCreate")
        sendEvent(jsonObject)
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Toast.makeText(this, "Service started", Toast.LENGTH_LONG).show()

        val jsonObject = JSONObject()
        jsonObject.put("Event", "onStart")
        sendEvent(jsonObject)

        return START_STICKY
    }

    private fun sendEvent(obj: JSONObject) {
        Log.i("MainService", "Sending obj: " + obj.toString())
        networkHandler?.obtainMessage()?.also {msg ->
            msg.obj = obj
            networkHandler?.sendMessage(msg)
        }

    }

    override fun onDestroy() {
        val jsonObject = JSONObject()
        jsonObject.put("Event", "onDestroy")
        sendEvent(jsonObject)
        Toast.makeText(this, "Service destroyed", Toast.LENGTH_LONG).show()
    }
}