import tkinter as tk
from tkinter import ttk
from pythonosc import udp_client

class OSCSliderApp:
    def __init__(self, root, num_sliders):
        self.root = root
        self.num_sliders = num_sliders
        self.clients = [None] * num_sliders

        self.osc_addresses = [tk.StringVar(value="127.0.0.1:8999") for _ in range(num_sliders)]
        self.osc_paths = [tk.StringVar(value=f"/BFI/MLAction/Action{i}") for i in range(num_sliders)]
        self.sliders = []

        self.setup_ui()

    def setup_ui(self):
        for i in range(self.num_sliders):
            frame = ttk.Frame(self.root)
            frame.pack(pady=10, fill=tk.X)

            # OSC Address input for each slider
            ttk.Label(frame, text=f"OSC Address for Slider {i+1} (ip:port):").pack(side=tk.LEFT)
            ttk.Entry(frame, textvariable=self.osc_addresses[i]).pack(side=tk.LEFT)

            # OSC Path input for each slider
            ttk.Label(frame, text=f"OSC Path for Slider {i+1}:").pack(side=tk.LEFT)
            ttk.Entry(frame, textvariable=self.osc_paths[i]).pack(side=tk.LEFT)

            # Slider
            slider = tk.Scale(frame, from_=0, to=1, resolution=0.01, orient=tk.HORIZONTAL, command=lambda val, idx=i: self.send_osc_message(idx, val))
            slider.pack(fill=tk.X, padx=10, pady=5)
            self.sliders.append(slider)

    def send_osc_message(self, slider_index, value):
        address = self.osc_addresses[slider_index].get()
        path = self.osc_paths[slider_index].get()
        if not address or not path:
            print(f"OSC address or path for slider {slider_index+1} is not set.")
            return

        if self.clients[slider_index] is None:
            try:
                ip, port = address.split(':')
                self.clients[slider_index] = udp_client.SimpleUDPClient(ip, int(port))
            except ValueError:
                print(f"Invalid OSC address format for slider {slider_index+1}. Expected format is ip:port.")
                return

        osc_path = path
        self.clients[slider_index].send_message(osc_path, float(value))
        print(f"Sent {value} to {osc_path}")

if __name__ == "__main__":
    root = tk.Tk()
    root.title("OSC Slider App")
    app = OSCSliderApp(root, num_sliders=8)  # Change the number of sliders as needed
    root.mainloop()