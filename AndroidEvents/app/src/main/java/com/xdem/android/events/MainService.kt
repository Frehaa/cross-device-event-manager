package com.xdem.android.events

import android.app.*
import android.content.Intent
import android.os.*
import android.util.Log
import android.widget.Toast
import org.json.JSONArray
import org.json.JSONObject
import java.io.OutputStream
import java.net.InetAddress
import java.net.Socket
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
            Looper.prepare()
            socket = Socket(address, port)
            networkHandler = NetworkHandler(socket!!.getOutputStream())
            Looper.loop()
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

        networkThread.start()
        val subscription = JSONObject()
        val events = JSONArray()
        events.put(getString(R.string.NEW_CONNECTION))
        subscription.put("Events", events)
        sendEvent(subscription)
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Toast.makeText(this, "Service started", Toast.LENGTH_LONG).show()
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
        Toast.makeText(this, "Service destroyed", Toast.LENGTH_LONG).show()
    }
}