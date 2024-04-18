using System.Diagnostics;
using System.Net;
using System.Text;

namespace JustProxies.XUnit;

public class WebServer
{
    private HttpListener httpListener = new HttpListener();

    public void Start(string url = "http://localhost:8880/")
    {
        httpListener.Prefixes.Add(url);
        httpListener.Start();
        var threadstart = new ThreadStart(() =>
        {
            while (httpListener.IsListening)
            {
                HttpListenerContext context = httpListener.GetContext();
                HttpListenerResponse response = context.Response;
                string responseData = context.Request.RawUrl.ToString();
                byte[] buffer = Encoding.UTF8.GetBytes(responseData);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
        });
        var _thread = new Thread(threadstart);
        _thread.Start();
    }

    public void Stop()
    {
        httpListener.Stop();
        httpListener.Close();
    }
}