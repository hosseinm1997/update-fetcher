# Telegram Bot Update Fetcher

This project will be useful when you are in development step of your bot project.

You can easily fetch bot updates from telegram servers and push it into any online/localhost address with your custom interval in miliseconds.

## Config
Just run Executables/FetchBotUpdate.exe and set your own config.

![Screenshot1](https://github.com/hosseinm1997/update-fetcher/raw/master/Screenshots/Screenshot1.jpg)


* Bot Token : Bot token achieved from https://t.me/botfather
* Url Address to send update : Local or online url address to send updates to
* Fetch updates every (in miliseconds): Interval to fetch updates from telegram servers.


**Note:** Updates will be sent via POST method with name **"update"**.



**For example see how to handle update sent to the url address in php**

```php
if(isset($_POST['update']))
{
    $update = $_POST['update'];
    $message = json_decode($update);
    
    $testfile = fopen("testfile.txt", 'a');
    fwrite($testfile,$message->from->first_name);
    fclose($testfile);
}
```


Good news is that you can have unlimited number of executables with different config file for each one of your bot projects because the config file be stored beside the exe file.



![Screenshot2](https://github.com/hosseinm1997/update-fetcher/raw/master/Screenshots/Screenshot2.jpg)
